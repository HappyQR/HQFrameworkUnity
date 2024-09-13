using System.Collections.Generic;
using HQFramework.Download;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private sealed class ResourceDownloader
        {
            private class DownloadItem : IReference
            {
                public string url { get; private set; }
                public string filePath { get; private set; }
                public AssetBundleInfo bundleInfo { get; private set; }

                public static DownloadItem Create(string url, string filePath, AssetBundleInfo bundleInfo)
                {
                    DownloadItem item = ReferencePool.Spawn<DownloadItem>();
                    item.url = url;
                    item.filePath = filePath;
                    item.bundleInfo = bundleInfo;
                    return item;
                }

                void IReference.OnRecyle()
                {
                    url = null;
                    filePath = null;
                    bundleInfo = null;
                }
            }

            private readonly ResourceManager resourceManager;
            private readonly IDownloadManager downloadManager;
            private readonly int hotfixDownloadGroupID;

            private Dictionary<int, DownloadItem> downloadDic;
            private Dictionary<int, Dictionary<int, DownloadItem>> downloadModuleMap;
            private Queue<AssetModuleInfo> downloadSuccessModuleQueue;

            public ResourceDownloader(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
                this.downloadManager = HQFrameworkEngine.GetModule<IDownloadManager>();
                this.hotfixDownloadGroupID = resourceManager.resourceHelper.HotfixDownloadGroupID;
            }

            public void StartHotfix()
            {
                foreach (KeyValuePair<AssetModuleInfo, List<AssetBundleInfo>> moduleBundleListPair in resourceManager.necessaryHotfixContent)
                {
                    AssetModuleInfo module = moduleBundleListPair.Key;
                    List<AssetBundleInfo> bundleList = moduleBundleListPair.Value;
                    for (int i = 0; i < bundleList.Count; i++)
                    {
                        string url = "";
                        string filePath = "";
                        DownloadItem item = DownloadItem.Create(url, filePath, bundleList[i]);
                        int downloadID = downloadManager.AddDownload(url, filePath, false, hotfixDownloadGroupID);
                        downloadManager.AddDownloadErrorEvent(downloadID, OnDownloadError);
                        downloadManager.AddDownloadCompleteEvent(downloadID, OnDownloadComplete);
                        downloadManager.AddDownloadPauseEvent(downloadID, OnDownloadPause);
                        downloadManager.AddDownloadResumeEvent(downloadID, OnDownloadResume);
                        downloadManager.AddDownloadUpdateEvent(downloadID, OnDownloadUpdate);

                        downloadDic.Add(downloadID, item);
                        if (!downloadModuleMap.ContainsKey(module.id))
                        {
                            downloadModuleMap.Add(module.id, new Dictionary<int, DownloadItem>());
                        }
                        downloadModuleMap[module.id].Add(downloadID, item);
                    }
                }
            }

            public void OnUpdate()
            {
                
            }

            public int DownloadModule(AssetModuleInfo module, List<AssetBundleInfo> bundleList)
            {
                return 0;
            }

            private void OnDownloadUpdate(DownloadUpdateEventArgs args)
            {

            }

            private void OnDownloadError(DownloadErrorEventArgs args)
            {

            }

            private void OnDownloadComplete(TaskInfo taskInfo)
            {

            }

            private void OnDownloadPause(TaskInfo taskInfo)
            {

            }

            private void OnDownloadResume(TaskInfo taskInfo)
            {

            }

            private void OnDownloadCancel(TaskInfo taskInfo)
            {

            }

            private void ClearHotfix() // clear after hotfix done.
            {
                
            }

            private void ClearDownload()
            {
                
            }
        }
    }
}
