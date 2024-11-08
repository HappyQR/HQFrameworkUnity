using System;
using System.Collections.Generic;
using System.IO;
using HQFramework.Download;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private sealed class ResourceDownloader
        {
            private class DownloadItem : IReference
            {
                public int hotfixID { get; private set; }
                public string url { get; private set; }
                public string filePath { get; private set; }
                public HQAssetBundleConfig bundleInfo { get; private set; }

                public static DownloadItem Create(int hotfixID, string url, string filePath, HQAssetBundleConfig bundleInfo)
                {
                    DownloadItem item = ReferencePool.Spawn<DownloadItem>();
                    item.hotfixID = hotfixID;
                    item.url = url;
                    item.filePath = filePath;
                    item.bundleInfo = bundleInfo;
                    return item;
                }

                void IReference.OnRecyle()
                {
                    hotfixID = -1;
                    url = null;
                    filePath = null;
                    bundleInfo = null;
                }
            }

            private class DownloadGroup
            {
                public int totalSize;
                public int downloadedSize;
                public int itemCount;
                public Queue<DownloadItem> downloadedQueue = new Queue<DownloadItem>();
            }

            private readonly ResourceManager resourceManager;
            private readonly string tempDownloadDir;

            private Dictionary<int, DownloadItem> downloadDic; // key: downloadID
            private Dictionary<int, DownloadGroup> downloadGroupDic; // key: downloadGroupID

            private Dictionary<int, Action<HotfixDownloadCancelEventArgs>> cancelEventDic;
            private Dictionary<int, Action<HotfixDownloadCompleteEventArgs>> completeEventDic;
            private Dictionary<int, Action<HotfixDownloadErrorEventArgs>> errorEventDic;
            private Dictionary<int, Action<HotfixDownloadPauseEventArgs>> pauseEventDic;
            private Dictionary<int, Action<HotfixDownloadResumeEventArgs>> resumeEventDic;
            private Dictionary<int, Action<HotfixDownloadUpdateEventArgs>> updateEventDic;

            public ResourceDownloader(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
                this.tempDownloadDir = Path.Combine(resourceManager.PersistentDir, "temp_download_cache");
                if (!Directory.Exists(tempDownloadDir))
                {
                    Directory.CreateDirectory(tempDownloadDir);
                }

                downloadDic = new Dictionary<int, DownloadItem>();
                downloadGroupDic = new Dictionary<int, DownloadGroup>();
                cancelEventDic = new Dictionary<int, Action<HotfixDownloadCancelEventArgs>>();
                completeEventDic = new Dictionary<int, Action<HotfixDownloadCompleteEventArgs>>();
                errorEventDic = new Dictionary<int, Action<HotfixDownloadErrorEventArgs>>();
                pauseEventDic = new Dictionary<int, Action<HotfixDownloadPauseEventArgs>>();
                resumeEventDic = new Dictionary<int, Action<HotfixDownloadResumeEventArgs>>();
                updateEventDic = new Dictionary<int, Action<HotfixDownloadUpdateEventArgs>>();
            }

            public void OnUpdate()
            {
                if (downloadGroupDic.Count > 0)
                {
                    foreach (int hotfixID in downloadGroupDic.Keys)
                    {
                        if (updateEventDic.ContainsKey(hotfixID))
                        {
                            DownloadGroup group = downloadGroupDic[hotfixID];
                            HotfixDownloadUpdateEventArgs args = HotfixDownloadUpdateEventArgs.Create(hotfixID, group.downloadedSize, group.totalSize);
                            updateEventDic[hotfixID].Invoke(args);
                            ReferencePool.Recyle(args);
                        }
                    }
                }
            }

            public int LaunchHotfix()
            {
                int hotfixID = resourceManager.resourceHelper.LauncherHotfixID;
                DownloadGroup downloadGroup = new DownloadGroup();
                downloadGroupDic.Add(hotfixID, downloadGroup);
                foreach (KeyValuePair<HQAssetModuleConfig, List<HQAssetBundleConfig>> moduleBundleListPair in resourceManager.necessaryHotfixContent)
                {
                    HQAssetModuleConfig module = moduleBundleListPair.Key;
                    List<HQAssetBundleConfig> bundleList = moduleBundleListPair.Value;

                    for (int i = 0; i < bundleList.Count; i++)
                    {
                        string url = Path.Combine(module.moduleUrlRoot, bundleList[i].bundleUrlRelatedToModule);
                        string filePath = GetBundleDownloadPath(bundleList[i]);
                        DownloadItem item = DownloadItem.Create(hotfixID, url, filePath, bundleList[i]);
                        AddDownloadItem(item);
                    }
                }
                return hotfixID;
            }

            public int ModuleHotfix(HQAssetModuleConfig module, List<HQAssetBundleConfig> bundleList)
            {
                int hotfixID = resourceManager.resourceHelper.LauncherHotfixID + module.id;
                DownloadGroup downloadGroup = new DownloadGroup();
                downloadGroupDic.Add(hotfixID, downloadGroup);
                for (int i = 0; i < bundleList.Count; i++)
                {
                    string url = Path.Combine(module.moduleUrlRoot, bundleList[i].bundleUrlRelatedToModule);
                    string filePath = GetBundleDownloadPath(bundleList[i]);
                    DownloadItem item = DownloadItem.Create(hotfixID, url, filePath, bundleList[i]);
                    AddDownloadItem(item);
                }
                return hotfixID;
            }

            private void AddDownloadItem(DownloadItem item)
            {
                downloadGroupDic[item.hotfixID].totalSize += item.bundleInfo.size;
                downloadGroupDic[item.hotfixID].itemCount++;
                int downloadID = resourceManager.downloadManager.AddDownload(item.url, item.filePath, true, item.hotfixID);
                resourceManager.downloadManager.AddDownloadErrorEvent(downloadID, OnDownloadError);
                resourceManager.downloadManager.AddDownloadCompleteEvent(downloadID, OnDownloadComplete);
                resourceManager.downloadManager.AddDownloadUpdateEvent(downloadID, OnDownloadUpdate);

                downloadDic.Add(downloadID, item);
            }

            public void PauseHotfix(int hotfixID)
            {
                int pauseCount = resourceManager.downloadManager.PauseDownloads(hotfixID);
                if (pauseCount > 0 && pauseEventDic.ContainsKey(hotfixID))
                {
                    HotfixDownloadPauseEventArgs args = HotfixDownloadPauseEventArgs.Create(hotfixID, pauseCount);
                    pauseEventDic[hotfixID]?.Invoke(args);
                    ReferencePool.Recyle(args);
                }
            }

            public void ResumeHotfix(int hotfixID)
            {
                int resumeCount = resourceManager.downloadManager.ResumeDownloads(hotfixID);
                if (resumeCount > 0 && resumeEventDic.ContainsKey(hotfixID))
                {
                    HotfixDownloadResumeEventArgs args = HotfixDownloadResumeEventArgs.Create(hotfixID, resumeCount);
                    resumeEventDic[hotfixID]?.Invoke(args);
                    ReferencePool.Recyle(args);
                }
            }

            public void CancelHotfix(int hotfixID)
            {
                int cancelCount = resourceManager.downloadManager.StopDownloads(hotfixID);
                if (cancelCount > 0 && cancelEventDic.ContainsKey(hotfixID))
                {
                    HotfixDownloadCancelEventArgs args = HotfixDownloadCancelEventArgs.Create(hotfixID, cancelCount);
                    cancelEventDic[hotfixID]?.Invoke(args);
                    ReferencePool.Recyle(args);
                }
                ClearHotfix(hotfixID);
            }

            public void AddHotfixDownloadUpdateEvent(int hotfixID, Action<HotfixDownloadUpdateEventArgs> onHotfixUpdate)
            {
                if (updateEventDic.ContainsKey(hotfixID))
                {
                    updateEventDic[hotfixID] += onHotfixUpdate;
                }
                else
                {
                    updateEventDic.Add(hotfixID, onHotfixUpdate);
                }
            }

            public void AddHotfixDownloadErrorEvent(int hotfixID, Action<HotfixDownloadErrorEventArgs> onHotfixError)
            {
                if (errorEventDic.ContainsKey(hotfixID))
                {
                    errorEventDic[hotfixID] += onHotfixError;
                }
                else
                {
                    errorEventDic.Add(hotfixID, onHotfixError);
                }
            }

            public void AddHotfixDownloadPauseEvent(int hotfixID, Action<HotfixDownloadPauseEventArgs> onHotfixPause)
            {
                if (pauseEventDic.ContainsKey(hotfixID))
                {
                    pauseEventDic[hotfixID] += onHotfixPause;
                }
                else
                {
                    pauseEventDic.Add(hotfixID, onHotfixPause);
                }
            }

            public void AddHotfixDownloadResumeEvent(int hotfixID, Action<HotfixDownloadResumeEventArgs> onHotfixResume)
            {
                if (resumeEventDic.ContainsKey(hotfixID))
                {
                    resumeEventDic[hotfixID] += onHotfixResume;
                }
                else
                {
                    resumeEventDic.Add(hotfixID, onHotfixResume);
                }
            }

            public void AddHotfixDownloadCancelEvent(int hotfixID, Action<HotfixDownloadCancelEventArgs> onHotfixCancel)
            {
                if (cancelEventDic.ContainsKey(hotfixID))
                {
                    cancelEventDic[hotfixID] += onHotfixCancel;
                }
                else
                {
                    cancelEventDic.Add(hotfixID, onHotfixCancel);
                }
            }

            public void AddHotfixDownloadCompleteEvent(int hotfixID, Action<HotfixDownloadCompleteEventArgs> onHotfixComplete)
            {
                if (completeEventDic.ContainsKey(hotfixID))
                {
                    completeEventDic[hotfixID] += onHotfixComplete;
                }
                else
                {
                    completeEventDic.Add(hotfixID, onHotfixComplete);
                }
            }

            private void OnDownloadUpdate(DownloadUpdateEventArgs args)
            {
                int hotfixID = args.GroupID;
                downloadGroupDic[hotfixID].downloadedSize += args.DeltaSize;
            }

            private void OnDownloadError(DownloadErrorEventArgs args)
            {
                resourceManager.downloadManager.StopDownloads(args.GroupID);
                int hotfixID = args.GroupID;
                HQAssetBundleConfig bundleInfo = downloadDic[args.ID].bundleInfo;
                if (errorEventDic.ContainsKey(hotfixID))
                {
                    HotfixDownloadErrorEventArgs errArgs = HotfixDownloadErrorEventArgs.Create(hotfixID, bundleInfo, args.ErrorMessage);
                    errorEventDic[hotfixID]?.Invoke(errArgs);
                    ReferencePool.Recyle(errArgs);
                }
                downloadDic.Remove(args.ID);
                downloadGroupDic.Remove(hotfixID);
                ClearHotfix(hotfixID);
            }

            private void OnDownloadComplete(TaskInfo taskInfo)
            {
                DownloadItem item = downloadDic[taskInfo.id];
                downloadDic.Remove(taskInfo.id);
                downloadGroupDic[item.hotfixID].itemCount--;
                // do the hash check
                string localHash = Utility.Hash.ComputeHash(item.filePath);
                if (localHash != item.bundleInfo.md5)
                {
                    // download again
                    AddDownloadItem(item);
                    return;
                }

                // bundle item download complete
                DownloadGroup group = downloadGroupDic[item.hotfixID];
                group.downloadedQueue.Enqueue(item);
                if (group.itemCount > 0)
                {
                    // continue download the rest of bundles
                    return;
                }
                // this group's download complete
                while (group.downloadedQueue.Count > 0)
                {
                    DownloadItem downloadItem = group.downloadedQueue.Dequeue();
                    string destBundlePath = GetBundleDestPath(downloadItem.bundleInfo);
                    string moduleDir = Path.GetDirectoryName(destBundlePath);
                    if (!Directory.Exists(moduleDir))
                    {
                        Directory.CreateDirectory(moduleDir);
                    }
                    File.Copy(downloadItem.filePath, destBundlePath, true);
                    File.Delete(downloadItem.filePath);
                }
                downloadGroupDic.Remove(item.hotfixID);
                resourceManager.ReloadAssetTable();

                // override local manifest
                if (item.hotfixID == resourceManager.resourceHelper.LauncherHotfixID) // launcher hotfix complete.
                {
                    DeleteObsoleteModules();
                    foreach (HQAssetModuleConfig remoteModule in resourceManager.necessaryHotfixContent.Keys)
                    {
                        MergeRemoteModuleToLocalManifest(remoteModule);
                    }
                    resourceManager.localManifest.productName = resourceManager.remoteManifest.productName;
                    resourceManager.localManifest.productVersion = resourceManager.remoteManifest.productVersion;
                    resourceManager.localManifest.resourceVersion = resourceManager.remoteManifest.resourceVersion;
                    resourceManager.localManifest.versionCode = resourceManager.remoteManifest.versionCode;
                    resourceManager.localManifest.minimalSupportedVersionCode = resourceManager.remoteManifest.minimalSupportedVersionCode;
                    resourceManager.localManifest.releaseNote = resourceManager.remoteManifest.releaseNote;
                    resourceManager.localManifest.isBuiltinManifest = false;
                    resourceManager.necessaryHotfixContent.Clear();
                    resourceManager.necessaryHotfixContent = null;
                }
                else                                                                // separate hotfix complete.
                {
                    HQAssetModuleConfig remoteModule = resourceManager.remoteManifest.moduleDic[item.bundleInfo.moduleID];
                    MergeRemoteModuleToLocalManifest(remoteModule);
                    resourceManager.separateHotfixContent.Remove(remoteModule);
                }

                if (completeEventDic.ContainsKey(item.hotfixID))
                {
                    HotfixDownloadCompleteEventArgs args = HotfixDownloadCompleteEventArgs.Create(item.hotfixID);
                    completeEventDic[item.hotfixID]?.Invoke(args);
                    ReferencePool.Recyle(args);
                    ClearHotfix(item.hotfixID);
                }

                if (downloadDic.Count == 0) // hotfix complete.
                {
                    resourceManager.resourceHelper.OverrideLocalManifest(resourceManager.localManifest);
                }
            }

            private void MergeRemoteModuleToLocalManifest(HQAssetModuleConfig remoteModule)
            {
                if (resourceManager.localManifest.moduleDic.ContainsKey(remoteModule.id))
                {
                    HQAssetModuleConfig localModule = resourceManager.localManifest.moduleDic[remoteModule.id];
                    // delete obsolete bundles
                    foreach (var bundle in localModule.bundleDic.Values)
                    {
                        if (!remoteModule.bundleDic.ContainsKey(bundle.bundleName))
                        {
                            string bundlePath = GetBundleDestPath(bundle);
                            if (File.Exists(bundlePath))
                            {
                                File.Delete(bundlePath);
                            }
                        }
                    }
                    
                    resourceManager.localManifest.moduleDic[remoteModule.id] = remoteModule;
                }
                else
                {
                    resourceManager.localManifest.moduleDic.Add(remoteModule.id, remoteModule);
                }
            }

            private void DeleteObsoleteModules()
            {
                List<int> obsoleteModuleList = new List<int>();
                foreach (HQAssetModuleConfig localModule in resourceManager.localManifest.moduleDic.Values)
                {
                    if (!resourceManager.remoteManifest.moduleDic.ContainsKey(localModule.id))
                    {
                        obsoleteModuleList.Add(localModule.id);
                        resourceManager.resourceHelper.DeleteAssetModule(localModule);
                    }
                }
                for (int i = 0; i < obsoleteModuleList.Count; i++)
                {
                    resourceManager.localManifest.moduleDic.Remove(obsoleteModuleList[i]);
                }
            }

            private string GetBundleDownloadPath(HQAssetBundleConfig bundleInfo)
            {
                return Path.Combine(tempDownloadDir, bundleInfo.md5);
            }

            private string GetBundleDestPath(HQAssetBundleConfig bundleInfo)
            {
                return Path.Combine(resourceManager.PersistentDir, resourceManager.resourceHelper.GetBundleRelatedPath(bundleInfo));
            }

            private void ClearHotfix(int hotfixID)
            {
                cancelEventDic.Remove(hotfixID);
                completeEventDic.Remove(hotfixID);
                errorEventDic.Remove(hotfixID);
                pauseEventDic.Remove(hotfixID);
                resumeEventDic.Remove(hotfixID);
                updateEventDic.Remove(hotfixID);
            }
        }
    }
}
