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
            private bool enableAutoHashCheck;
            private HttpClient client;
            private DownloadTaskWorker worker;

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

            public static DownloadTask Create(HttpClient client, string url, string filePath, bool resumable, bool enableAutoHashCheck, int groupID, int priority)
            {
                DownloadTask task = ReferencePool.Spawn<DownloadTask>();
                task.id = serialID++;
                task.client = client;
                task.groupID = groupID;
                task.priority = priority;
                task.url = url;
                task.filePath = filePath;
                task.resumable = resumable;
                task.enableAutoHashCheck = enableAutoHashCheck;
                return task;
            }

            public override TaskStartStatus Start()
            {
                worker = DownloadTaskWorker.Create(this);
                worker.HashCheckEvent += onHashCheck;
                worker.DownloadUpdateEvent += onDownloadUpdate;
                worker.CompleteEvent += OnDownloadComplete;
                worker.DownloadErrorEvent += OnDownloadError;

                worker.Start(client, url, filePath, resumable, enableAutoHashCheck);

                status = TaskStatus.InProgress;
                return TaskStartStatus.InProgress;
            }

            public override void OnUpdate()
            {
                
            }

            private void OnDownloadError(DownloadErrorEventArgs e)
            {
                status = TaskStatus.Error;
                onDownloadError?.Invoke(e);
            }

            private void OnDownloadComplete()
            {
                status = TaskStatus.Done;
                TaskInfo taskInfo = new TaskInfo(id, groupID, priority, status);
                onCompleted?.Invoke(taskInfo);
            }

            public override void OnRecyle()
            {
                base.OnRecyle();
                client = null;
                url = null;
                filePath = null;
                resumable = false;
                enableAutoHashCheck = false;
                onHashCheck = null;
                onDownloadError = null;
                onDownloadUpdate = null;

                if (worker != null)
                {
                    ReferencePool.Recyle(worker);
                    worker = null;
                }
            }
        }
    }
}
