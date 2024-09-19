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
                public AssetBundleInfo bundleInfo { get; private set; }

                public static DownloadItem Create(int hotfixID, string url, string filePath, AssetBundleInfo bundleInfo)
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
            }

            private readonly ResourceManager resourceManager;
            private readonly IDownloadManager downloadManager;

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
                this.downloadManager = HQFrameworkEngine.GetModule<IDownloadManager>();

                downloadDic = new Dictionary<int, DownloadItem>();
                downloadGroupDic = new Dictionary<int, DownloadGroup>();
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
                foreach (KeyValuePair<AssetModuleInfo, List<AssetBundleInfo>> moduleBundleListPair in resourceManager.necessaryHotfixContent)
                {
                    AssetModuleInfo module = moduleBundleListPair.Key;
                    List<AssetBundleInfo> bundleList = moduleBundleListPair.Value;

                    string moduleUrlRoot = Path.Combine(resourceManager.remoteManifest.hotfixUrlRoot, module.moduleUrlRelatedToHotfixUrlRoot);
                    for (int i = 0; i < bundleList.Count; i++)
                    {
                        string url = Path.Combine(moduleUrlRoot, bundleList[i].bundleUrlRelatedToModule);
                        string filePath = resourceManager.GetBundleFilePath(bundleList[i]);
                        DownloadItem item = DownloadItem.Create(hotfixID, url, filePath, bundleList[i]);
                        AddDownloadItem(item);
                    }
                }
                return hotfixID;
            }

            public int ModuleHotfix(AssetModuleInfo module, List<AssetBundleInfo> bundleList)
            {
                int hotfixID = resourceManager.resourceHelper.LauncherHotfixID + module.id;
                DownloadGroup downloadGroup = new DownloadGroup();
                downloadGroupDic.Add(hotfixID, downloadGroup);
                string moduleUrlRoot = Path.Combine(resourceManager.remoteManifest.hotfixUrlRoot, module.moduleUrlRelatedToHotfixUrlRoot);
                for (int i = 0; i < bundleList.Count; i++)
                {
                    string url = Path.Combine(moduleUrlRoot, bundleList[i].bundleUrlRelatedToModule);
                    string filePath = resourceManager.GetBundleFilePath(bundleList[i]);
                    DownloadItem item = DownloadItem.Create(hotfixID, url, filePath, bundleList[i]);
                    AddDownloadItem(item);
                }
                return hotfixID;
            }

            private void AddDownloadItem(DownloadItem item)
            {
                downloadGroupDic[item.hotfixID].totalSize += item.bundleInfo.size;
                downloadGroupDic[item.hotfixID].itemCount++;
                int downloadID = downloadManager.AddDownload(item.url, item.filePath, false, item.hotfixID);
                downloadManager.AddDownloadErrorEvent(downloadID, OnDownloadError);
                downloadManager.AddDownloadCompleteEvent(downloadID, OnDownloadComplete);
                downloadManager.AddDownloadUpdateEvent(downloadID, OnDownloadUpdate);

                downloadDic.Add(downloadID, item);
            }

            public void PauseHotfix(int hotfixID)
            {
                int pauseCount = downloadManager.PauseDownloads(hotfixID);
                if (pauseCount > 0 && pauseEventDic.ContainsKey(hotfixID))
                {
                    HotfixDownloadPauseEventArgs args = HotfixDownloadPauseEventArgs.Create(hotfixID, pauseCount);
                    pauseEventDic[hotfixID]?.Invoke(args);
                    ReferencePool.Recyle(args);
                }
            }

            public void ResumeHotfix(int hotfixID)
            {
                int resumeCount = downloadManager.ResumeDownloads(hotfixID);
                if (resumeCount > 0 && resumeEventDic.ContainsKey(hotfixID))
                {
                    HotfixDownloadResumeEventArgs args = HotfixDownloadResumeEventArgs.Create(hotfixID, resumeCount);
                    resumeEventDic[hotfixID]?.Invoke(args);
                    ReferencePool.Recyle(args);
                }
            }

            public void CancelHotfix(int hotfixID)
            {
                int cancelCount = downloadManager.StopDownloads(hotfixID);
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
                downloadManager.StopDownloads(args.GroupID);
                int hotfixID = args.GroupID;
                AssetBundleInfo bundleInfo = downloadDic[hotfixID].bundleInfo;
                if (errorEventDic.ContainsKey(hotfixID))
                {
                    HotfixDownloadErrorEventArgs errArgs = HotfixDownloadErrorEventArgs.Create(hotfixID, bundleInfo, args.ErrorMessage);
                    errorEventDic[hotfixID]?.Invoke(errArgs);
                    ReferencePool.Recyle(errArgs);
                }
                ClearHotfix(hotfixID);
            }

            private void OnDownloadComplete(TaskInfo taskInfo)
            {
                DownloadItem item = downloadDic[taskInfo.id];
                downloadDic.Remove(taskInfo.id);
                downloadGroupDic[item.hotfixID].itemCount--;
                // do the hash check
                string localHash = Utility.Hash.ComputeHash(item.filePath);
                if (localHash == item.bundleInfo.md5)
                {
                    // hash check passed
                    if (downloadGroupDic[item.hotfixID].itemCount > 0)
                    {
                        // continue.
                        return;
                    }
                    //this group's download complete.
                    downloadGroupDic.Remove(item.hotfixID);
                    if (item.hotfixID == resourceManager.resourceHelper.LauncherHotfixID) // launcher hotfix complete.
                    {
                        DeleteObsoleteModules();
                        foreach (AssetModuleInfo remoteModule in resourceManager.necessaryHotfixContent.Keys)
                        {
                            MergeRemoteModuleToLocalManifest(remoteModule);
                        }
                        resourceManager.localManifest.productName = resourceManager.remoteManifest.productName;
                        resourceManager.localManifest.productVersion = resourceManager.remoteManifest.productVersion;
                        resourceManager.localManifest.runtimePlatform = resourceManager.remoteManifest.runtimePlatform;
                        resourceManager.localManifest.resourceVersion = resourceManager.remoteManifest.resourceVersion;
                        resourceManager.localManifest.minimalSupportedVersion = resourceManager.remoteManifest.minimalSupportedVersion;
                        resourceManager.localManifest.releaseNote = resourceManager.remoteManifest.releaseNote;
                        resourceManager.localManifest.hotfixUrlRoot = resourceManager.remoteManifest.hotfixUrlRoot;
                        resourceManager.localManifest.isBuiltinManifest = false;
                        resourceManager.necessaryHotfixContent.Clear();
                        resourceManager.necessaryHotfixContent = null;
                    }
                    else                                                                // separate hotfix complete.
                    {
                        AssetModuleInfo remoteModule = resourceManager.remoteManifest.moduleDic[item.bundleInfo.moduleID];
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
                }
                else
                {
                    // redownload
                    AddDownloadItem(item);
                }

                if (downloadDic.Count == 0) // hotfix complete.
                {
                    resourceManager.resourceHelper.OverrideLocalManifest(resourceManager.localManifest);
                }
            }

            private void MergeRemoteModuleToLocalManifest(AssetModuleInfo remoteModule)
            {
                if (resourceManager.localManifest.moduleDic.ContainsKey(remoteModule.id))
                {
                    AssetModuleInfo localModule = resourceManager.localManifest.moduleDic[remoteModule.id];
                    // delete obsolete bundles
                    foreach (var bundle in localModule.bundleDic.Values)
                    {
                        if (!remoteModule.bundleDic.ContainsKey(bundle.bundleName))
                        {
                            string bundlePath = resourceManager.GetBundleFilePath(bundle);
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
                foreach (AssetModuleInfo localModule in resourceManager.localManifest.moduleDic.Values)
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