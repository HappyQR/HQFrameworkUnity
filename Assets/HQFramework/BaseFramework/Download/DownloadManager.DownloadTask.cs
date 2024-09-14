using System;
using System.Net.Http;

namespace HQFramework.Download
{
    internal sealed partial class DownloadManager
    {
        private sealed class DownloadTask : ResumableTask
        {
            private static int serialID;
            
            private string url;
            private string filePath;
            private bool resumable; //broken-point continuingly-transferring
            private int lastDownloadedSize = 0;
            private HttpClient client;
            private DownloadTaskWorker worker;
            private DownloadTaskSignal signal;

            private Action<DownloadErrorEventArgs> onDownloadError;
            private Action<DownloadUpdateEventArgs> onDownloadUpdate;

            public event Action<DownloadErrorEventArgs> DownloadErrorEvent
            {
                add { onDownloadError += value;}
                remove { onDownloadError -= value;}
            }

            public event Action<DownloadUpdateEventArgs> DownloadUpdateEvent
            {
                add { onDownloadUpdate += value;}
                remove { onDownloadUpdate -= value;}
            }

            public static DownloadTask Create(HttpClient client, string url, string filePath, bool resumable, int groupID, int priority)
            {
                DownloadTask task = ReferencePool.Spawn<DownloadTask>();
                task.id = serialID++;
                task.client = client;
                task.groupID = groupID;
                task.priority = priority;
                task.url = url;
                task.filePath = filePath;
                task.resumable = resumable;
                return task;
            }

            public override TaskStartStatus Start()
            {
                worker = DownloadTaskWorker.Create(this);
                worker.Start(client, url, filePath, resumable);
                status = TaskStatus.InProgress;
                return TaskStartStatus.InProgress;
            }

            public override void OnUpdate()
            {
                if (signal != null)
                {
                    if (signal.Result == DownloadResult.Complete)
                    {
                        int deltaSize = signal.DownloadedSize - lastDownloadedSize;
                        DownloadUpdateEventArgs args = DownloadUpdateEventArgs.Create(id, groupID, url, filePath, deltaSize, signal.DownloadedSize, signal.TotalSize);
                        onDownloadUpdate?.Invoke(args);
                        ReferencePool.Recyle(args);

                        status = TaskStatus.Done;
                        TaskInfo taskInfo = new TaskInfo(id, groupID, priority, status);
                        onCompleted?.Invoke(taskInfo);
                    }
                    else if (signal.Result == DownloadResult.Canceled)
                    {
                        status = TaskStatus.Canceled;
                        TaskInfo taskInfo = new TaskInfo(id, groupID, priority, status);
                        onCancel?.Invoke(taskInfo);
                    }
                    else if (signal.Result == DownloadResult.Error)
                    {
                        status = TaskStatus.Error;
                        DownloadErrorEventArgs args = DownloadErrorEventArgs.Create(id, groupID, url, filePath, signal.ErrorMessage);
                        onDownloadError?.Invoke(args);
                        ReferencePool.Recyle(args);
                    }
                    ReferencePool.Recyle(signal);
                    return;
                }

                // handle the update event
                if (worker.Status == TaskStatus.InProgress)
                {
                    int deltaSize = worker.DownloadedSize - lastDownloadedSize;
                    lastDownloadedSize = worker.DownloadedSize;
                    DownloadUpdateEventArgs args = DownloadUpdateEventArgs.Create(id, groupID, url, filePath, deltaSize, worker.DownloadedSize, worker.TotalSize);
                    onDownloadUpdate?.Invoke(args);
                    ReferencePool.Recyle(args);
                } 
            }

            public override void Cancel()
            {
                if (worker != null)
                {
                    worker.Cancel();
                }
            }

            public override bool Pause()
            {
                if (worker != null && worker.Pause())
                {
                    return base.Pause();
                }
                return false;
            }

            public override bool Resume()
            {
                if (worker!= null && worker.Resume())
                {
                    return base.Resume();
                }
                return false;
            }

            public void ReceiveSignal(DownloadTaskSignal signal)
            {
                this.signal = signal;
            }

            protected override void OnRecyle()
            {
                base.OnRecyle();
                lastDownloadedSize = 0;
                client = null;
                url = null;
                filePath = null;
                worker = null;
                resumable = false;
                onDownloadError = null;
                onDownloadUpdate = null;
            }
        }
    }
}
