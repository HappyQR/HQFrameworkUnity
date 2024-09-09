using System;
using System.Net.Http;

namespace HQFramework.Download
{
    internal sealed partial class DownloadManager : HQModuleBase, IDownloadManager
    {
        private HttpClient client;
        private DownloadTaskDispatcher dispatcher;

        public override byte Priority => byte.MaxValue;

        public void InitDownloadModule(float timeOut, ushort maxConcurrentCount)
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeOut);
            dispatcher = new DownloadTaskDispatcher(maxConcurrentCount);
        }

        protected override void OnUpdate()
        {
            dispatcher.ProcessTasks();
        }

        protected override void OnShutdown()
        {
            client.CancelPendingRequests();
            dispatcher.CancelAllTasks();
        }

        public int AddDownload(string url, string filePath, bool resumable = false, int groupID = 0, int priority = 0)
        {
            DownloadTask task = DownloadTask.Create(client, url, filePath, resumable, groupID, priority);
            return dispatcher.AddTask(task);
        }

        public void AddDownloadCancelEvent(int id, Action<TaskInfo> onCancel)
        {
            dispatcher.AddTaskCancelEvent(id, onCancel);
        }

        public void AddDownloadCompleteEvent(int id, Action<TaskInfo> onCompleted)
        {
            dispatcher.AddTaskCompleteEvent(id, onCompleted);
        }

        public void AddDownloadPauseEvent(int id, Action<TaskInfo> onPause)
        {
            dispatcher.AddTaskPauseEvent(id, onPause);
        }

        public void AddDownloadResumeEvent(int id, Action<TaskInfo> onResume)
        {
            dispatcher.AddTaskResumeEvent(id, onResume);
        }

        public void AddDownloadErrorEvent(int id, Action<DownloadErrorEventArgs> onError)
        {
            dispatcher.AddDownloadErrorEvent(id, onError);
        }

        public void AddDownloadUpdateEvent(int id, Action<DownloadUpdateEventArgs> onDownloadUpdate)
        {
            dispatcher.AddDownloadUpdateEvent(id, onDownloadUpdate);
        }

        public void AddDownloadHashCheckEvent(int id, Action<DownloadHashCheckEventArgs> onHashCheck)
        {
            dispatcher.AddDownloadHashCheckEvent(id, onHashCheck);
        }

        public bool PauseDownload(int id)
        {
            return dispatcher.PauseTask(id);
        }

        public bool ResumeDownload(int id)
        {
            return dispatcher.ResumeTask(id);
        }

        public bool StopDownload(int id)
        {
            return dispatcher.CancelTask(id);
        }

        public int StopDownloads(int groupID)
        {
            return dispatcher.CancelTasks(groupID);
        }

        public int PauseDownloads(int groupID)
        {
            return dispatcher.PauseTasks(groupID);
        }

        public int ResumeDownloads(int groupID)
        {
            return dispatcher.ResumeTasks(groupID);
        }

        public void StopAllDownloads()
        {
            dispatcher.CancelAllTasks();
        }
    }
}
