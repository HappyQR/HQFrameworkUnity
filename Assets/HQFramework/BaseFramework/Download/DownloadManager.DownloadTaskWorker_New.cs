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
        private class DownloadTaskWorker_New : IReference
        {
            private byte[] buffer;
            private int totalSize;
            private int downloadedSize;
            private string errorMessage;
            private TaskStatus status;
            private DownloadTask_New task;
            private HttpResponseMessage responseMsg;
            private CancellationTokenSource cancelToken;
            private BlockingCollection<byte[]> writeQueue;

            private static readonly int defaultBufferSize = 262144; //256kb

            public TaskStatus Status => status;
            public int TotalSize => totalSize;
            public int DownloadedSize => downloadedSize;

            public static DownloadTaskWorker_New Create(DownloadTask_New task)
            {
                DownloadTaskWorker_New worker = ReferencePool.Spawn<DownloadTaskWorker_New>();
                worker.buffer = ArrayPool<byte>.Shared.Rent(defaultBufferSize);
                worker.cancelToken = new CancellationTokenSource();
                worker.task = task;
                return worker;
            }

            public void Start(HttpClient client, string url, string filePath, bool resumable)
            {
                Task.Run(() => StartInternalAsync(client, url, filePath, resumable));
            }

            private async void StartInternalAsync(HttpClient client, string url, string filePath, bool resumable)
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
                                status = TaskStatus.Done;
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
                            return;
                        }
                        catch (Exception ex)
                        {
                            status = TaskStatus.Error;
                            errorMessage = ex.Message;
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
                    using Stream netStream = await responseMsg.Content.ReadAsStreamAsync();
                    while (true)
                    {
                        if (status == TaskStatus.Canceled) // call stop manually
                        {
                            break;
                        }
                        else if (status == TaskStatus.Paused)
                        {
                            await Task.Yield();
                            continue;
                        }
                        receivedSize = netStream.Read(buffer, bufferOffset, defaultBufferSize - bufferOffset);
                        if (receivedSize == 0)
                        {
                            byte[] extraDataChunck = new byte[bufferOffset];
                            Array.Copy(buffer, 0, extraDataChunck, 0, bufferOffset);
                            writeQueue.Add(extraDataChunck);
                            status = TaskStatus.Done;
                            break;
                        }
                        bufferOffset += receivedSize;
                        downloadedSize += receivedSize;
                        if (bufferOffset == defaultBufferSize)
                        {
                            byte[] tempDataChunck = ArrayPool<byte>.Shared.Rent(bufferOffset);
                            Array.Copy(buffer, 0, tempDataChunck, 0, bufferOffset);
                            writeQueue.Add(tempDataChunck);
                            bufferOffset = 0;
                        }
                    }
                }
                catch (TaskCanceledException)
                {

                }
                catch (Exception ex)
                {
                    status = TaskStatus.Error;
                    errorMessage = ex.Message;
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
                }
            }

            public bool Pause()
            {
                if (status == TaskStatus.InProgress)
                {
                    status = TaskStatus.Paused;
                    return true;
                }
                return false;
            }

            public bool Resume()
            {
                if (status == TaskStatus.Paused)
                {
                    status = TaskStatus.InProgress;
                    return true;
                }
                return false;
            }

            void IReference.OnRecyle()
            {
                totalSize = downloadedSize = 0;
                responseMsg?.Dispose();
                cancelToken.Dispose();
                ArrayPool<byte>.Shared.Return(buffer);
                buffer = null;
                responseMsg = null;
                cancelToken = null;
                status = TaskStatus.Waiting;
                errorMessage = null;
            }
        }
    }
}
