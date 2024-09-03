using System;

namespace HQFramework.Download
{
    public interface IDownloadManager
    {
        void InitDownloadModule(float timeOut, ushort maxConcurrentCount);

        int AddDownload(string url, string filePath, bool resumable = false, bool enableAutoHashCheck = false, int groupID = 0, int priority = 0);

        void AddDownloadCancelEvent(int id, Action<TaskInfo> onCancel);

        void AddDownloadCompleteEvent(int id, Action<TaskInfo> onCompleted);

        public void AddDownloadPauseEvent(int id, Action<TaskInfo> onPause);

        public void AddDownloadResumeEvent(int id, Action<TaskInfo> onResume);

        public void AddDownloadErrorEvent(int id, Action<DownloadErrorEventArgs> onError);

        public void AddDownloadUpdateEvent(int id, Action<DownloadUpdateEventArgs> onDpwnloadUpdate);

        public void AddDownloadHashCheckEvent(int id, Action<DownloadHashCheckEventArgs> onHashCheck);

        bool PauseDownload(int id);

        bool ResumeDownload(int id);

        int StopDownloads(int groupID);

        int PauseDownloads(int groupID);

        int ResumeDownloads(int groupID);

        void StopAllDownloads();
    }
}
