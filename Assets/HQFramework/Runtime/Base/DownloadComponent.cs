using System;
using HQFramework.Download;

namespace HQFramework.Runtime
{
    public class DownloadComponent : BaseComponent
    {
        public float timeOut = 10f;
        public ushort maxConcurrentCount = 5;

        private IDownloadManager downloadManager;

        private void Start()
        {
            downloadManager = HQFrameworkEngine.GetModule<IDownloadManager>();
            downloadManager.InitDownloadModule(timeOut, maxConcurrentCount);
        }

        public int AddDownload(string url, string filePath, bool resumable = false, int groupID = 0, int priority = 0)
        {
            return downloadManager.AddDownload(url, filePath, resumable, groupID, priority);
        }

        public void AddDownloadCancelEvent(int id, Action<TaskInfo> onCancel)
        {
            downloadManager.AddDownloadCancelEvent(id, onCancel);
        }

        public void AddDownloadCompleteEvent(int id, Action<TaskInfo> onCompleted)
        {
            downloadManager.AddDownloadCompleteEvent(id, onCompleted);
        }

        public void AddDownloadErrorEvent(int id, Action<DownloadErrorEventArgs> onError)
        {
            downloadManager.AddDownloadErrorEvent(id, onError);
        }

        public void AddDownloadPauseEvent(int id, Action<TaskInfo> onPause)
        {
            downloadManager.AddDownloadPauseEvent(id, onPause);
        }

        public void AddDownloadResumeEvent(int id, Action<TaskInfo> onResume)
        {
            downloadManager.AddDownloadResumeEvent(id, onResume);
        }

        public void AddDownloadUpdateEvent(int id, Action<DownloadUpdateEventArgs> onDownloadUpdate)
        {
            downloadManager.AddDownloadUpdateEvent(id, onDownloadUpdate);
        }

        public bool PauseDownload(int id)
        {
            return downloadManager.PauseDownload(id);
        }

        public int PauseDownloads(int groupID)
        {
            return downloadManager.PauseDownloads(groupID);
        }

        public bool ResumeDownload(int id)
        {
            return downloadManager.ResumeDownload(id);
        }

        public int ResumeDownloads(int groupID)
        {
            return downloadManager.ResumeDownloads(groupID);
        }

        public void StopAllDownloads()
        {
            downloadManager.StopAllDownloads();
        }

        public bool StopDownload(int id)
        {
            return downloadManager.StopDownload(id);
        }

        public int StopDownloads(int groupID)
        {
            return downloadManager.StopDownloads(groupID);
        }
    }
}
