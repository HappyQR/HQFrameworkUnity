using System;

namespace HQFramework.Download
{
    internal sealed partial class DownloadManager
    {
        private sealed class DownloadTaskDispatcher : ResumableTaskDispatcher<DownloadTask>
        {
            public DownloadTaskDispatcher(ushort maxConcurrentCount) : base(maxConcurrentCount)
            {
            }

            public void AddDownloadErrorEvent(int id, Action<DownloadErrorEventArgs> onDownloadError)
            {
                if (taskDic.ContainsKey(id))
                {
                    taskDic[id].DownloadErrorEvent += onDownloadError;
                }
            }

            public void AddDownloadUpdateEvent(int id, Action<DownloadUpdateEventArgs> onDownloadUpdate)
            {
                if (taskDic.ContainsKey(id))
                {
                    taskDic[id].DownloadUpdateEvent += onDownloadUpdate;
                }
            }
        }
    }
}
