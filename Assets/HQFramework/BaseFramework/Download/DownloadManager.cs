using System;
using System.Net.Http;

namespace HQFramework.Download
{
    internal sealed partial class DownloadManager : HQModuleBase, IDownloadManager
    {
        private HttpClient client;
        private DownloadTaskDispatcher dispatcher;

        public override byte Priority => byte.MaxValue;

        protected override void OnUpdate()
        {
            dispatcher.ProcessTasks();
        }

        protected override void OnShutdown()
        {
            client.CancelPendingRequests();
        }

        public void InitDownloadModule(float timeOut, ushort maxConcurrentCount)
        {
            throw new NotImplementedException();
        }

        public int AddDownload(string url, string filePath, bool resumable = false, bool enableAutoHashCheck = false, int groupID = 0, int priority = 0)
        {
            throw new NotImplementedException();
        }

        public void AddDownloadCancelEvent(int id, Action<TaskInfo> onCancel)
        {
            throw new NotImplementedException();
        }

        public void AddDownloadCompleteEvent(int id, Action<TaskInfo> onCompleted)
        {
            throw new NotImplementedException();
        }

        public void AddDownloadPauseEvent(int id, Action<TaskInfo> onPause)
        {
            throw new NotImplementedException();
        }

        public void AddDownloadResumeEvent(int id, Action<TaskInfo> onResume)
        {
            throw new NotImplementedException();
        }

        public void AddDownloadErrorEvent(int id, Action<DownloadErrorEventArgs> onError)
        {
            throw new NotImplementedException();
        }

        public void AddDownloadUpdateEvent(int id, Action<DownloadUpdateEventArgs> onDpwnloadUpdate)
        {
            throw new NotImplementedException();
        }

        public void AddDownloadHashCheckEvent(int id, Action<DownloadHashCheckEventArgs> onHashCheck)
        {
            throw new NotImplementedException();
        }

        public bool PauseDownload(int id)
        {
            throw new NotImplementedException();
        }

        public bool ResumeDownload(int id)
        {
            throw new NotImplementedException();
        }

        public int StopDownloads(int groupID)
        {
            throw new NotImplementedException();
        }

        public int PauseDownloads(int groupID)
        {
            throw new NotImplementedException();
        }

        public int ResumeDownloads(int groupID)
        {
            throw new NotImplementedException();
        }

        public void StopAllDownloads()
        {
            throw new NotImplementedException();
        }
    }
}
