using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HQFramework.Download
{
    internal partial class DownloadManager
    {
        private class DownloadTaskWorker : IReference
        {
            private byte[] buffer;
            private int totalSize;
            private int downloadedSize;
            private DownloadTask task;
            private HttpResponseMessage responseMsg;

            private Action<DownloadHashCheckEventArgs> onHashCheck;
            private Action<DownloadErrorEventArgs> onDownloadError;
            private Action<DownloadUpdateEventArgs> onDownloadUpdate;
            private Action onCompleted;

            private static readonly int defaultBufferSize = 131072; // 128kb

            public event Action<DownloadHashCheckEventArgs> HashCheckEvent
            {
                add { onHashCheck += value; }
                remove { onHashCheck -= value; }
            }

            public event Action<DownloadErrorEventArgs> DownloadErrorEvent
            {
                add { onDownloadError += value; }
                remove { onDownloadError -= value; }
            }

            public event Action<DownloadUpdateEventArgs> DownloadUpdateEvent
            {
                add { onDownloadUpdate += value; }
                remove { onDownloadUpdate -= value; }
            }

            public event Action CompleteEvent
            {
                add { onCompleted += value; }
                remove { onCompleted -= value; }
            }

            public static DownloadTaskWorker Create(DownloadTask task)
            {
                DownloadTaskWorker worker = ReferencePool.Spawn<DownloadTaskWorker>();
                worker.task = task;
                if (worker.buffer == null)
                {
                    worker.buffer = new byte[defaultBufferSize];
                }
                return worker;
            }

            public async void Start(HttpClient client, string url, string filePath, bool resumable, bool enableAutoHashCheck)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                using HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Get, url);
                long rangeOffset = 0;
                if (fileInfo.Exists)
                {
                    if (resumable)
                    {
                        try
                        {
                            using HttpRequestMessage contentLengtRequestMsg = new HttpRequestMessage(HttpMethod.Head, url);
                            using HttpResponseMessage contentLengthResponseMsg = await client.SendAsync(contentLengtRequestMsg);
                            contentLengthResponseMsg.EnsureSuccessStatusCode();
                            totalSize = (int)contentLengthResponseMsg.Content.Headers.ContentLength.Value;
                            rangeOffset = fileInfo.Length;
                            if (rangeOffset > totalSize)
                            {
                                // localFile.size > remoteFile.size, just delete local file to redownload
                                fileInfo.Delete();
                            }
                            else if (rangeOffset == totalSize)
                            {
                                // maybe download complete, just do the hash checking.
                                if (enableAutoHashCheck)
                                {
                                    byte[] md5 = contentLengthResponseMsg.Content.Headers.ContentMD5;
                                    if (md5 != null && md5.Length > 0)
                                    {
                                        string targetHash = Utility.Hash.ConvertByHashBytes(md5);
                                        string hash = Utility.Hash.ComputeHash(fileInfo.OpenRead());
                                        DownloadHashCheckEventArgs args = DownloadHashCheckEventArgs.Create(task.ID, task.GroupID, url, filePath, targetHash, hash, hash == targetHash);
                                        onHashCheck?.Invoke(args);
                                        ReferencePool.Recyle(args);
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("AutoHashCheck is not supported on target server!");
                                    }
                                }
                                onCompleted?.Invoke();
                                return;
                            }
                            else
                            {
                                // download not complete
                                requestMsg.Headers.Range = new RangeHeaderValue(rangeOffset, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (task.Status != TaskStatus.Canceled) // not call stop manually
                            {
                                DownloadErrorEventArgs errAgrs = DownloadErrorEventArgs.Create(task.ID, task.GroupID, url, filePath, ex.Message);
                                onDownloadError?.Invoke(errAgrs);
                                ReferencePool.Recyle(errAgrs);
                                responseMsg?.Dispose();
                            }
                            return;
                        }
                    }
                    else
                    {
                        fileInfo.Delete();
                    }
                }
                using FileStream fs = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Seek(fs.Length, SeekOrigin.Begin);
                downloadedSize += (int)fs.Length;
                int receivedSize = 0;
                int bufferOffset = 0;
                try
                {
                    responseMsg = await client.SendAsync(requestMsg, HttpCompletionOption.ResponseHeadersRead);
                    responseMsg?.EnsureSuccessStatusCode();
                    totalSize = (int)(responseMsg.Content.Headers.ContentLength.Value + rangeOffset);
                    DownloadUpdateEventArgs downloadUpdateArgs = DownloadUpdateEventArgs.Create(task.ID, task.GroupID, url, filePath, downloadedSize, downloadedSize, totalSize);
                    onDownloadUpdate?.Invoke(downloadUpdateArgs);
                    ReferencePool.Recyle(downloadUpdateArgs);
                    using Stream netStream = await responseMsg.Content.ReadAsStreamAsync();
                    while (true)
                    {
                        if (task.Status == TaskStatus.Paused)
                        {
                            await Task.Yield();
                            continue;
                        }
                        else if (task.Status == TaskStatus.Canceled) // call stop manually
                        {
                            responseMsg.Dispose();
                            return;
                        }
                        receivedSize = await netStream.ReadAsync(buffer, bufferOffset, buffer.Length - bufferOffset);
                        if (receivedSize == 0)
                        {
                            await fs.WriteAsync(buffer, 0, bufferOffset);
                            downloadedSize += bufferOffset;
                            downloadUpdateArgs = DownloadUpdateEventArgs.Create(task.ID, task.GroupID, url, filePath, bufferOffset, downloadedSize, totalSize);
                            onDownloadUpdate?.Invoke(downloadUpdateArgs);
                            ReferencePool.Recyle(downloadUpdateArgs);
                            break;
                        }
                        bufferOffset += receivedSize;
                        if (bufferOffset == buffer.Length)
                        {
                            await fs.WriteAsync(buffer, 0, bufferOffset);
                            downloadedSize += bufferOffset;
                            downloadUpdateArgs = DownloadUpdateEventArgs.Create(task.ID, task.GroupID, url, filePath, bufferOffset, downloadedSize, totalSize);
                            onDownloadUpdate?.Invoke(downloadUpdateArgs);
                            ReferencePool.Recyle(downloadUpdateArgs);
                            bufferOffset = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (task.Status != TaskStatus.Canceled) // not call stop manually
                    {
                        DownloadErrorEventArgs errAgrs = DownloadErrorEventArgs.Create(task.ID, task.GroupID, url, filePath, ex.Message);
                        onDownloadError?.Invoke(errAgrs);
                        ReferencePool.Recyle(errAgrs);
                        responseMsg?.Dispose();
                    }
                    return;
                }
                if (enableAutoHashCheck)
                {
                    byte[] md5 = responseMsg.Content.Headers.ContentMD5;
                    if (md5 != null && md5.Length > 0)
                    {
                        string targetHash = Utility.Hash.ConvertByHashBytes(md5);
                        fs.Seek(0, SeekOrigin.Begin);
                        string hash = Utility.Hash.ComputeHash(fs);
                        DownloadHashCheckEventArgs args = DownloadHashCheckEventArgs.Create(task.ID, task.GroupID, url, filePath, targetHash, hash, hash == targetHash);
                        onHashCheck?.Invoke(args);
                        ReferencePool.Recyle(args);
                    }
                    else
                    {
                        responseMsg?.Dispose();
                        throw new InvalidOperationException("AutoHashCheck is not supported on target server!");
                    }
                }
                onCompleted?.Invoke();
                responseMsg?.Dispose();
            }

            public void OnRecyle()
            {
                totalSize = downloadedSize = 0;
                task = null;
                onHashCheck = null;
                onDownloadError = null;
                onDownloadUpdate = null;
                onCompleted = null;
                responseMsg = null;
            }
        }
    }
}
