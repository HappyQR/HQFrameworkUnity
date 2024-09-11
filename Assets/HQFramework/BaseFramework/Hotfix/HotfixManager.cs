using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using HQFramework.Download;
using HQFramework.Resource;

namespace HQFramework.Hotfix
{
    internal sealed partial class HotfixManager : HQModuleBase, IHotfixManager
    {
        private static readonly byte hotfixTimeout = 10;
        private static readonly int hotfixDownloadGroupID = 1;

        private ResourceConfig config;
        private AssetModuleManifest localManifest;
        private AssetModuleManifest remoteManifest;
        private List<HotfixPatch> patchList;
        private Dictionary<int, HotfixDownloadItem> downloadDic;
        private Dictionary<int, Dictionary<int, HotfixDownloadItem>> moduleDownloadMap;
        private float totalSize;
        private float downloadedSize;
        private IDownloadManager downloadManager;
        private string manifestFilePath;

        public override byte Priority => byte.MaxValue;

        public event Action<HotfixCheckEventArgs> onHotfixCheckDone;
        public event Action<HotfixCheckErrorEventArgs> onHotfixCheckError;
        public event Action<HotfixErrorEventArgs> onHotfixError;
        public event Action<HotfixUpdateEventArgs> onHotfixUpdate;
        public event Action onHotfixDone;

        protected override void OnInitialize()
        {
            config = HQFrameworkEngine.GetModule<IResourceManager>().Config;
            if (config == null)
            {
                throw new InvalidOperationException("You need to initialize Resource Module before using Hotfix Module.");
            }

            this.manifestFilePath = Path.Combine(config.assetPersistentDir, ResourceManager.manifestFileName);
            this.localManifest = SerializeManager.JsonToObject<AssetModuleManifest>(File.ReadAllText(manifestFilePath));
        }

        public void StartHotfix()
        {
            if (patchList == null || patchList.Count == 0)
            {
                onHotfixDone?.Invoke();
                ClearHotfix();
                return;
            }
            downloadManager = HQFrameworkEngine.GetModule<IDownloadManager>();
            downloadDic = new Dictionary<int, HotfixDownloadItem>();
            moduleDownloadMap = new Dictionary<int, Dictionary<int, HotfixDownloadItem>>();
            string hotfixUrlRoot = Path.Combine(config.hotfixUrl, remoteManifest.resourceVersion.ToString());
            for (int i = 0; i < patchList.Count; i++)
            {
                AssetModuleInfo module = patchList[i].module;
                string moduleUrlRoot = Path.Combine(hotfixUrlRoot, module.moduleName, module.currentPatchVersion.ToString());
                string moduleLocalDir = Path.Combine(config.assetPersistentDir, module.moduleName);
                if (!Directory.Exists(moduleLocalDir))
                {
                    Directory.CreateDirectory(moduleLocalDir);
                }
                moduleDownloadMap.Add(module.id, new Dictionary<int, HotfixDownloadItem>());
                for (int j = 0; j < patchList[i].bundleList.Count; j++)
                {
                    AssetBundleInfo bundle = patchList[i].bundleList[j];
                    string bundleUrl = Path.Combine(moduleUrlRoot, bundle.bundleName);
                    string bundlePath = Path.Combine(moduleLocalDir, bundle.bundleName);
                    int downloadID = downloadManager.AddDownload(bundleUrl, bundlePath, false, hotfixDownloadGroupID, 0);
                    downloadManager.AddDownloadErrorEvent(downloadID, OnDownloadBundleError);
                    downloadManager.AddDownloadUpdateEvent(downloadID, OnDownloadBundleUpdate);
                    downloadManager.AddDownloadCompleteEvent(downloadID, OnDownloadBundleDone);
                    HotfixDownloadItem item = new HotfixDownloadItem(bundleUrl, bundlePath, bundle);
                    downloadDic.Add(downloadID, item);
                    moduleDownloadMap[module.id].Add(downloadID, item);
                }
            }
        }

        public async void StartHotfixCheck()
        {
            try
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(hotfixTimeout);
                string jsonStr = await client.GetStringAsync(config.hotfixManifestUrl);
                remoteManifest = SerializeManager.JsonToObject<AssetModuleManifest>(jsonStr);
                Type helperType = Type.GetType($"HQFramework.Hotfix.{config.hotfixMode}Checker");
                IHotfixChecker checker = Activator.CreateInstance(helperType) as IHotfixChecker;
                HotfixCheckEventArgs args = checker.CheckManifestUpdate(localManifest, remoteManifest);
                patchList = args.patchList;
                totalSize = args.totalSize;
                onHotfixCheckDone?.Invoke(args);
            }
            catch (Exception ex)
            {
                HotfixCheckErrorEventArgs args = new HotfixCheckErrorEventArgs(ex.Message);
                onHotfixCheckError?.Invoke(args);
            }
        }

        private void OnDownloadBundleError(DownloadErrorEventArgs args)
        {
            downloadManager.StopDownloads(hotfixDownloadGroupID);
            HotfixErrorEventArgs errorArgs = new HotfixErrorEventArgs(args.ErrorMsg);
            onHotfixError?.Invoke(errorArgs);
            SaveLocalManifest();
        }

        private void OnDownloadBundleDone(TaskInfo taskInfo)
        {
            HotfixDownloadItem item = downloadDic[taskInfo.id];
            downloadDic.Remove(taskInfo.id);
            moduleDownloadMap[item.bundle.moduleID].Remove(taskInfo.id);
            // do the hash check.
            string localHash = Utility.Hash.ComputeHash(item.filePath);
            if (localHash == item.bundle.md5)
            {
                // hash check passed.
                if (moduleDownloadMap[item.bundle.moduleID].Count == 0)
                {
                    AssetModuleInfo remoteModule = remoteManifest.moduleDic[item.bundle.moduleID];
                    if (localManifest.moduleDic.ContainsKey(remoteModule.id))
                    {
                        // delete obsolete bundles
                        foreach (AssetBundleInfo localBundle in localManifest.moduleDic[remoteModule.id].bundleDic.Values)
                        {
                            if (!remoteModule.bundleDic.ContainsKey(localBundle.bundleName))
                            {
                                string obsoleteBundlePath = Path.Combine(config.assetPersistentDir, remoteModule.moduleName, localBundle.bundleName);
                                if (File.Exists(obsoleteBundlePath))
                                {
                                    File.Delete(obsoleteBundlePath);
                                }
                            }
                        }
                        localManifest.moduleDic[remoteModule.id] = remoteModule;
                    }
                    else
                    {
                        localManifest.moduleDic.Add(remoteModule.id, remoteModule);
                    }
                    SaveLocalManifest();
                }
            }
            else
            {
                // redownload
                int downloadID = downloadManager.AddDownload(item.url, item.filePath, false, hotfixDownloadGroupID, 0);
                downloadManager.AddDownloadErrorEvent(downloadID, OnDownloadBundleError);
                downloadManager.AddDownloadUpdateEvent(downloadID, OnDownloadBundleUpdate);
                downloadManager.AddDownloadCompleteEvent(downloadID, OnDownloadBundleDone);
                downloadDic.Add(downloadID, item);
                moduleDownloadMap[item.bundle.moduleID].Add(downloadID, item);
            }

            if (downloadDic.Count == 0)
            {
                // hotfix done.
                // override localManifest.
                DeleteObsoleteAssets();
                localManifest.productName = remoteManifest.productName;
                localManifest.productVersion = remoteManifest.productVersion;
                localManifest.runtimePlatform = remoteManifest.runtimePlatform;
                localManifest.resourceVersion = remoteManifest.resourceVersion;
                localManifest.minimalSupportedVersion = remoteManifest.minimalSupportedVersion;
                localManifest.releaseNote = remoteManifest.releaseNote;
                localManifest.isBuiltinManifest = false;
                SaveLocalManifest();

                onHotfixDone?.Invoke();
                ClearHotfix();
            }
        }

        private void OnDownloadBundleUpdate(DownloadUpdateEventArgs args)
        {
            downloadedSize += args.DeltaSize;
            HotfixUpdateEventArgs updateArgs = HotfixUpdateEventArgs.Create(downloadedSize / totalSize);
            onHotfixUpdate?.Invoke(updateArgs);
            ReferencePool.Recyle(updateArgs);
        }

        private void DeleteObsoleteAssets()
        {
            foreach (AssetModuleInfo localModule in localManifest.moduleDic.Values)
            {
                if (!remoteManifest.moduleDic.ContainsKey(localModule.id))
                {
                    string localModuleDir = Path.Combine(config.assetPersistentDir, localModule.moduleName);
                    if (Directory.Exists(localModuleDir))
                    {
                        Directory.Delete(localModuleDir, true);
                    }
                }
            }
        }

        private void SaveLocalManifest()
        {
            string manifestJsonStr = SerializeManager.ObjectToJson(localManifest);
            File.WriteAllText(manifestFilePath, manifestJsonStr);
        }

        private void ClearHotfix()
        {
            downloadManager = null;
            onHotfixCheckDone = null;
            onHotfixCheckError = null;
            onHotfixError = null;
            onHotfixUpdate = null;
            onHotfixDone = null;
            patchList = null;
            downloadDic = null;
            moduleDownloadMap = null;
            manifestFilePath = null;
            downloadedSize = totalSize = 0;
        }

        protected override void OnShutdown()
        {
            ClearHotfix();
            config = null;
            localManifest = null;
            remoteManifest = null;
        }
    }
}
