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

            private static readonly int defaultBufferSize = 262144; //256kb

            public TaskStatus Status => status;
            public int TotalSize => totalSize;
            public int DownloadedSize => downloadedSize;

            public static DownloadTaskWorker Create(DownloadTask task)
            {
                DownloadTaskWorker worker = ReferencePool.Spawn<DownloadTaskWorker>();
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
                                DownloadTaskSignal signal = DownloadTaskSignal.Create(true, null, totalSize, totalSize);
                                task.ReceiveSignal(signal);
                                ReferencePool.Recyle(this);
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
                            status = TaskStatus.Error;
                            DownloadTaskSignal signal = DownloadTaskSignal.Create(false, ex.Message, downloadedSize, totalSize);
                            task.ReceiveSignal(signal);
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
                    using Stream netStream = await responseMsg.Content.ReadAsStreamAsync();
                    status = TaskStatus.InProgress;
                    while (true)
                    {
                        if (status == TaskStatus.Canceled) // call stop manually
                        {
                            status = TaskStatus.Canceled;
                            DownloadTaskSignal signal = DownloadTaskSignal.Create(false, "Task was canceled.", downloadedSize, totalSize);
                            task.ReceiveSignal(signal);
                            ReferencePool.Recyle(this);
                            break;
                        }
                        else if (status == TaskStatus.Paused)
                        {
                            continue;
                        }
                        receivedSize = netStream.Read(buffer, bufferOffset, defaultBufferSize - bufferOffset);
                        if (receivedSize == 0)
                        {
                            byte[] extraDataChunck = new byte[bufferOffset];
                            Array.Copy(buffer, 0, extraDataChunck, 0, bufferOffset);
                            writeQueue.Add(extraDataChunck);
                            writeQueue.CompleteAdding();
                            await writeTask;
                            status = TaskStatus.Done;
                            DownloadTaskSignal signal = DownloadTaskSignal.Create(true, null, downloadedSize, totalSize);
                            task.ReceiveSignal(signal);
                            ReferencePool.Recyle(this);
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
                catch (Exception ex)
                {
                    writeQueue.CompleteAdding();
                    await writeTask;
                    status = TaskStatus.Error;
                    DownloadTaskSignal signal = DownloadTaskSignal.Create(false, ex.Message, downloadedSize, totalSize);
                    task.ReceiveSignal(signal);
                    ReferencePool.Recyle(this);
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
                if (status == TaskStatus.Waiting || status == TaskStatus.InProgress || status == TaskStatus.Paused)
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
                ArrayPool<byte>.Shared.Return(buffer);
                buffer = null;
                totalSize = downloadedSize = 0;
                status = TaskStatus.Waiting;
                task = null;
                responseMsg?.Dispose();
                cancelToken.Dispose();
                responseMsg = null;
                cancelToken = null;
            }
        }
    }
}
