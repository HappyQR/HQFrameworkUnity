using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
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
            private TaskStatus status;
            private DownloadTask task;
            private HttpResponseMessage responseMsg;
            private CancellationTokenSource cancelToken;
            private BlockingCollection<byte[]> writeQueue;

            private Action<DownloadHashCheckEventArgs> onHashCheck;
            private Action<DownloadErrorEventArgs> onDownloadError;
            private Action<DownloadUpdateEventArgs> onDownloadUpdate;
            private Action onPaused;
            private Action onResume;
            private Action onCompleted;
            private Action onCanceled;

            private static readonly int defaultBufferSize = 262144; //256kb

            public TaskStatus Status => status;

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

            public event Action CancelEvent
            {
                add { onCanceled += value; }
                remove { onCanceled -= value; }
            }

            public event Action PauseEvent
            {
                add { onPaused += value; }
                remove {onPaused -= value; }
            }

            public event Action ResumeEvent
            {
                add { onResume += value; }
                remove { onResume -= value; }
            }

            public static DownloadTaskWorker Create(DownloadTask task)
            {
                DownloadTaskWorker worker = ReferencePool.Spawn<DownloadTaskWorker>();
                worker.buffer = ArrayPool<byte>.Shared.Rent(defaultBufferSize);
                worker.cancelToken = new CancellationTokenSource();
                worker.task = task;
                worker.status = TaskStatus.Waiting;
                return worker;
            }

            public async void Start(HttpClient client, string url, string filePath, bool resumable, bool enableAutoHashCheck)
            {
                status = TaskStatus.InProgress;

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
                            using HttpResponseMessage contentLengthResponseMsg = await client.SendAsync(contentLengtRequestMsg, cancelToken.Token);
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
                                        string hash = Utility.Hash.ComputeHash(filePath);
                                        DownloadHashCheckEventArgs args = DownloadHashCheckEventArgs.Create(task.ID, task.GroupID, url, filePath, targetHash, hash, hash == targetHash);
                                        onHashCheck?.Invoke(args);
                                        ReferencePool.Recyle(args);
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("AutoHashCheck is not supported on target server!");
                                    }
                                }
                                status = TaskStatus.Done;
                                onCompleted?.Invoke();
                                ReferencePool.Recyle(this);
                                return;
                            }
                            else
                            {
                                // download not complete
                                requestMsg.Headers.Range = new RangeHeaderValue(rangeOffset, null);
                            }
                        }
                        catch (TaskCanceledException) // canceled
                        {
                            ReferencePool.Recyle(this);
                            return;
                        }
                        catch (Exception ex)
                        {
                            status = TaskStatus.Error;
                            DownloadErrorEventArgs errAgrs = DownloadErrorEventArgs.Create(task.ID, task.GroupID, url, filePath, ex.Message);
                            onDownloadError?.Invoke(errAgrs);
                            ReferencePool.Recyle(errAgrs);
                            ReferencePool.Recyle(this);
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
                writeQueue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
                Task writeTask = Task.Run(() => ProcessBytesWriteAsync(fs)); 
                try
                {
                    responseMsg = await client.SendAsync(requestMsg, HttpCompletionOption.ResponseHeadersRead, cancelToken.Token);
                    responseMsg.EnsureSuccessStatusCode();
                    totalSize = (int)(responseMsg.Content.Headers.ContentLength.Value + rangeOffset);
                    DownloadUpdateEventArgs downloadUpdateArgs = DownloadUpdateEventArgs.Create(task.ID, task.GroupID, url, filePath, downloadedSize, downloadedSize, totalSize);
                    onDownloadUpdate?.Invoke(downloadUpdateArgs);
                    ReferencePool.Recyle(downloadUpdateArgs);
                    using Stream netStream = await responseMsg.Content.ReadAsStreamAsync();
                    while (true)
                    {
                        if (status == TaskStatus.Canceled) // call stop manually
                        {
                            ReferencePool.Recyle(this);
                            break;
                        }
                        else if (status == TaskStatus.Paused)
                        {
                            await Task.Yield();
                            continue;
                        }
                        receivedSize = await netStream.ReadAsync(buffer, bufferOffset, defaultBufferSize - bufferOffset, cancelToken.Token);
                        if (receivedSize == 0)
                        {
                            // await fs.WriteAsync(buffer, 0, bufferOffset, cancelToken.Token);

                            byte[] extraDataChunck = new byte[bufferOffset];
                            Array.Copy(buffer, 0, extraDataChunck, 0, bufferOffset);
                            writeQueue.Add(extraDataChunck);

                            downloadedSize += bufferOffset;
                            downloadUpdateArgs = DownloadUpdateEventArgs.Create(task.ID, task.GroupID, url, filePath, bufferOffset, downloadedSize, totalSize);
                            onDownloadUpdate?.Invoke(downloadUpdateArgs);
                            ReferencePool.Recyle(downloadUpdateArgs);

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
                                    throw new InvalidOperationException("AutoHashCheck is not supported on target server!");
                                }
                            }
                            status = TaskStatus.Done;
                            onCompleted?.Invoke();
                            ReferencePool.Recyle(this);
                            break;
                        }
                        bufferOffset += receivedSize;
                        if (bufferOffset == defaultBufferSize)
                        {
                            // await fs.WriteAsync(buffer, 0, bufferOffset, cancelToken.Token);

                            byte[] tempDataChunck = ArrayPool<byte>.Shared.Rent(bufferOffset);
                            Array.Copy(buffer, 0, tempDataChunck, 0, bufferOffset);
                            writeQueue.Add(tempDataChunck);

                            downloadedSize += bufferOffset;
                            downloadUpdateArgs = DownloadUpdateEventArgs.Create(task.ID, task.GroupID, url, filePath, bufferOffset, downloadedSize, totalSize);
                            onDownloadUpdate?.Invoke(downloadUpdateArgs);
                            ReferencePool.Recyle(downloadUpdateArgs);
                            bufferOffset = 0;
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    ReferencePool.Recyle(this);
                }
                catch (Exception ex)
                {
                    status = TaskStatus.Error;
                    DownloadErrorEventArgs errAgrs = DownloadErrorEventArgs.Create(task.ID, task.GroupID, url, filePath, ex.Message);
                    onDownloadError?.Invoke(errAgrs);
                    ReferencePool.Recyle(errAgrs);
                    responseMsg?.Dispose();
                }
                finally
                {
                    writeQueue.CompleteAdding();
                    await writeTask;
                }
            }

            private void ProcessBytesWriteAsync(FileStream fs)
            {
                foreach (byte[] chunk in writeQueue.GetConsumingEnumerable())
                {
                    if (chunk.Length < defaultBufferSize)
                    {
                        fs.Write(chunk, 0, chunk.Length);
                    }
                    else
                    {
                        fs.Write(chunk, 0, defaultBufferSize);
                        ArrayPool<byte>.Shared.Return(chunk);
                    }
                }
                fs.Dispose();
                writeQueue.Dispose();
                writeQueue = null;
            }

            public void Cancel()
            {
                if (status == TaskStatus.InProgress || status == TaskStatus.Paused)
                {
                    status = TaskStatus.Canceled;
                    cancelToken.Cancel();
                    onCanceled?.Invoke();
                }
            }

            public void Pause()
            {
                if (status == TaskStatus.InProgress)
                {
                    status = TaskStatus.Paused;
                    onPaused.Invoke();
                }
            }

            public void Resume()
            {
                if (status == TaskStatus.Paused)
                {
                    status = TaskStatus.InProgress;
                    onResume.Invoke();
                }
            }

            void IReference.OnRecyle()
            {
                totalSize = downloadedSize = 0;
                responseMsg?.Dispose();
                cancelToken.Dispose();
                ArrayPool<byte>.Shared.Return(buffer);
                buffer = null;
                task = null;
                responseMsg = null;
                cancelToken = null;
                onCanceled = null;
                onCompleted = null;
                onPaused = null;
                onResume = null;
                onHashCheck = null;
                onDownloadError = null;
                onDownloadUpdate = null;
            }
        }
    }
}
