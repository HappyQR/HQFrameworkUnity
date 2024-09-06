using System;
using System.Net.Http;

namespace HQFramework.Download
{
    internal sealed partial class DownloadManager
    {
        private sealed class DownloadTask_New : ResumableTask
        {
            private static int serialID;
            
            private string url;
            private string filePath;
            private bool resumable; //broken-point continuingly-transferring
            private int lastDownloadedSize = 0;
            private HttpClient client;
            private DownloadTaskWorker_New worker;

            private Action<DownloadHashCheckEventArgs> onHashCheck;
            private Action<DownloadErrorEventArgs> onDownloadError;
            private Action<DownloadUpdateEventArgs> onDownloadUpdate;

            public event Action<DownloadHashCheckEventArgs> HashCheckEvent
            {
                add { onHashCheck += value;}
                remove { onHashCheck -= value;}
            }

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

            public static DownloadTask_New Create(HttpClient client, string url, string filePath, bool resumable, int groupID, int priority)
            {
                DownloadTask_New task = ReferencePool.Spawn<DownloadTask_New>();
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
                worker = DownloadTaskWorker_New.Create(this);
                worker.Start(client, url, filePath, resumable);

                return TaskStartStatus.InProgress;
            }

            public override void OnUpdate()
            {
                status = worker.Status;
            }

            private void OnDownloadComplete()
            {
                TaskInfo taskInfo = new TaskInfo(id, groupID, priority, status);
                onCompleted?.Invoke(taskInfo);
            }

            public override void Cancel()
            {
                if (worker != null)
                {
                    worker.Cancel();
                }
            }

            public override void Pause()
            {
                if (worker != null && worker.Pause())
                {
                    base.Pause();
                }
            }

            public override void Resume()
            {
                if (worker!= null && worker.Resume())
                {
                    base.Resume();
                }
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
                onHashCheck = null;
                onDownloadError = null;
                onDownloadUpdate = null;
            }
        }
    }
}
