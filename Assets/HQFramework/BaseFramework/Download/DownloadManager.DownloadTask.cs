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
                worker.DownloadErrorEvent += onDownloadError;
                worker.PauseEvent += OnDownloadPause;
                worker.ResumeEvent += OnDownloadResume;
                worker.CompleteEvent += OnDownloadComplete;
                worker.CancelEvent += OnDownloadCanceled;

                worker.Start(client, url, filePath, resumable, enableAutoHashCheck);

                return TaskStartStatus.InProgress;
            }

            public override void OnUpdate()
            {
                status = worker.Status;
            }

            private void OnDownloadPause()
            {
                TaskInfo taskInfo = new TaskInfo(id, groupID, priority, status);
                onPause?.Invoke(taskInfo);
            }

            private void OnDownloadResume()
            {
                TaskInfo taskInfo = new TaskInfo(id, groupID, priority, status);
                onResume?.Invoke(taskInfo);
            }

            private void OnDownloadComplete()
            {
                TaskInfo taskInfo = new TaskInfo(id, groupID, priority, status);
                onCompleted?.Invoke(taskInfo);
            }

            private void OnDownloadCanceled()
            {
                TaskInfo info = new TaskInfo(ID, GroupID, Priority, Status);
                onCancel?.Invoke(info);
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
                if (worker != null)
                {
                    worker.Pause();
                }
            }

            public override void Resume()
            {
                if (worker!= null)
                {
                    worker.Resume();
                }
            }

            protected override void OnRecyle()
            {
                base.OnRecyle();
                client = null;
                url = null;
                filePath = null;
                worker = null;
                resumable = false;
                enableAutoHashCheck = false;
                onHashCheck = null;
                onDownloadError = null;
                onDownloadUpdate = null;
            }
        }
    }
}
