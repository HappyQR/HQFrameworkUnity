using System;
using System.Collections.Generic;
using System.IO;
using HQFramework.Download;

namespace HQFramework.Resource
{
    internal sealed partial class ResourceManager : HQModuleBase, IResourceManager
    {
        private IResourceHelper resourceHelper;
        private IDownloadManager downloadManager;
        private ResourceHotfixChecker hotfixChecker;
        private ResourceDownloader resourceDownloader;
        private ResourceLoader resourceLoader;
        private BundleLoader bundleLoader;

        private HQAssetManifest localManifest;
        private HQAssetManifest remoteManifest;
        private Dictionary<HQAssetModuleConfig, List<HQAssetBundleConfig>> necessaryHotfixContent;
        private Dictionary<HQAssetModuleConfig, List<HQAssetBundleConfig>> separateHotfixContent;

        private Dictionary<string, string> bundleFilePathMap;
        private Dictionary<uint, HQAssetItemConfig> assetItemMap;
        private Dictionary<string, BundleItem> loadedBundleMap;
        private HashSet<BundleItem> loadingBundleSet;
        private Dictionary<uint, object> crcLoadedAssetMap; // key: crc, value: asset object
        private Dictionary<object, uint> loadedAssetCrcMap; // key: asset object, value: crc
        private Dictionary<object, object> instantiatedAssetMap; // key: instantiated Asset, value: origin asset

        public override byte Priority => byte.MaxValue;
        public string PersistentDir => resourceHelper.AssetsPersistentDir;
        public string BuiltinDir => resourceHelper.AssetsBuiltinDir;

        private static readonly ushort maxConcurrentLoadCount = 1024;

        protected override void OnInitialize()
        {
            bundleFilePathMap = new Dictionary<string, string>();
            assetItemMap = new Dictionary<uint, HQAssetItemConfig>();
            loadedBundleMap = new Dictionary<string, BundleItem>();
            loadingBundleSet = new HashSet<BundleItem>();
            crcLoadedAssetMap = new Dictionary<uint, object>();
            loadedAssetCrcMap = new Dictionary<object, uint>();
            instantiatedAssetMap = new Dictionary<object, object>();
        }

        protected override void OnUpdate()
        {
            bundleLoader?.OnUpdate();
            resourceLoader?.OnUpdate();
            resourceDownloader?.OnUpdate();
        }

        public void SetHelper(IResourceHelper resourceHelper)
        {
            this.resourceHelper = resourceHelper;
            hotfixChecker = new ResourceHotfixChecker(this);
            resourceLoader = new ResourceLoader(this);
            bundleLoader = new BundleLoader(this);

            localManifest = resourceHelper.LoadAssetManifest();
            ReloadAssetMap();
        }

        public void SetDownloadManager(IDownloadManager downloadManager)
        {
            this.downloadManager = downloadManager;
            resourceDownloader = new ResourceDownloader(this);
        }

        public int LaunchHotfixCheck()
        {
            if (resourceHelper.HotfixMode == HQHotfixMode.NoHotfix)
            {
                throw new InvalidOperationException("You can't use CheckHotfix under NoHotfix mode.");
            }

            return hotfixChecker.LaunchHotfix();
        }

        public int ModuleHotfixCheck(int moduleID)
        {
            if (resourceHelper.HotfixMode != HQHotfixMode.SeparateHotfix)
            {
                throw new InvalidOperationException("CheckModuleHotfix() only adapt to SeparateHotfix mode.");
            }

            return hotfixChecker.ModuleHotfixCheck(moduleID);
        }

        public void AddHotfixCheckErrorEvent(int hotfixID, Action<HotfixCheckErrorEventArgs> onHotfixCheckError)
        {
            hotfixChecker.AddHotfixCheckErrorEvent(hotfixID, onHotfixCheckError);
        }

        public void AddHotfixCheckCompleteEvent(int hotfixID, Action<HotfixCheckCompleteEventArgs> onHotfixCheckComplete)
        {
            hotfixChecker.AddHotfixCheckCompleteEvent(hotfixID, onHotfixCheckComplete);
        }

        public int LaunchHotfix()
        {
            if (resourceHelper.HotfixMode == HQHotfixMode.NoHotfix)
            {
                throw new InvalidOperationException("You can't use CheckHotfix under NoHotfix mode.");
            }
            if (necessaryHotfixContent == null || necessaryHotfixContent.Count == 0)
            {
                throw new InvalidOperationException("Nothing to update.");
            }
            
            return resourceDownloader.LaunchHotfix();
        }

        public int ModuleHotfix(int moduleID)
        {
            if (resourceHelper.HotfixMode != HQHotfixMode.SeparateHotfix)
            {
                throw new InvalidOperationException("CheckModuleHotfix() only adapt to SeparateHotfix mode.");
            }

            HQAssetModuleConfig remoteModule = remoteManifest.moduleDic[moduleID];
            if (separateHotfixContent.ContainsKey(remoteModule))
            {
                return resourceDownloader.ModuleHotfix(remoteModule, separateHotfixContent[remoteModule]);
            }
            else
            {
                throw new InvalidOperationException("Nothing to update");
            }
        }

        public void PauseHotfix(int hotfixID)
        {
            resourceDownloader.PauseHotfix(hotfixID);
        }

        public void ResumeHotfix(int hotfixID)
        {
            resourceDownloader.ResumeHotfix(hotfixID);
        }

        public void CancelHotfix(int hotfixID)
        {
            resourceDownloader.CancelHotfix(hotfixID);
        }

        public void AddHotfixDownloadUpdateEvent(int hotfixID, Action<HotfixDownloadUpdateEventArgs> onHotfixUpdate)
        {
            resourceDownloader.AddHotfixDownloadUpdateEvent(hotfixID, onHotfixUpdate);
        }

        public void AddHotfixDownloadErrorEvent(int hotfixID, Action<HotfixDownloadErrorEventArgs> onHotfixError)
        {
            resourceDownloader.AddHotfixDownloadErrorEvent(hotfixID, onHotfixError);
        }

        public void AddHotfixDownloadPauseEvent(int hotfixID, Action<HotfixDownloadPauseEventArgs> onHotfixPause)
        {
            resourceDownloader.AddHotfixDownloadPauseEvent(hotfixID, onHotfixPause);
        }

        public void AddHotfixDownloadResumeEvent(int hotfixID, Action<HotfixDownloadResumeEventArgs> onHotfixResume)
        {
            resourceDownloader.AddHotfixDownloadResumeEvent(hotfixID, onHotfixResume);
        }

        public void AddHotfixDownloadCancelEvent(int hotfixID, Action<HotfixDownloadCancelEventArgs> onHotfixCancel)
        {
            resourceDownloader.AddHotfixDownloadCancelEvent(hotfixID, onHotfixCancel);
        }

        public void AddHotfixDownloadCompleteEvent(int hotfixID, Action<HotfixDownloadCompleteEventArgs> onHotfixComplete)
        {
            resourceDownloader.AddHotfixDownloadCompleteEvent(hotfixID, onHotfixComplete);
        }

        public bool HasModule(int moduleID)
        {
            return localManifest.moduleDic.ContainsKey(moduleID);
        }

        public void LoadAsset(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            resourceLoader.LoadAsset(crc, assetType, onComplete, onError, priority, groupID);
        }

        public void LoadAsset<T>(uint crc, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class
        {
            resourceLoader.LoadAsset<T>(crc, onComplete, onError, priority, groupID);
        }

        public void LoadAsset(string path, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            uint crc = Utility.CRC32.ComputeCrc32(path);
            resourceLoader.LoadAsset(crc, assetType, onComplete, onError, priority, groupID);
        }

        public void LoadAsset<T>(string path, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class
        {
            uint crc = Utility.CRC32.ComputeCrc32(path);
            resourceLoader.LoadAsset<T>(crc, onComplete, onError, priority, groupID);
        }

        public object InstantiateAsset(object asset)
        {
            if (!loadedAssetCrcMap.ContainsKey(asset))
            {
                throw new InvalidOperationException("you can only instantiate object loaded from resource manager!");
            }

            object target = resourceHelper.InstantiateAsset(asset);
            instantiatedAssetMap.Add(target, asset);
            string bundleName = assetItemMap[loadedAssetCrcMap[asset]].bundleName;
            loadedBundleMap[bundleName].refCount++;
            return target;
        }

        public T InstantiateAsset<T>(T asset) where T : class
        {
            if (!loadedAssetCrcMap.ContainsKey(asset))
            {
                throw new InvalidOperationException("you can only instantiate object loaded from resource manager!");
            }

            object target = resourceHelper.InstantiateAsset(asset);
            instantiatedAssetMap.Add(target, asset);
            string bundleName = assetItemMap[loadedAssetCrcMap[asset]].bundleName;
            loadedBundleMap[bundleName].refCount++;
            return (T)target;
        }

        public void ReleaseAsset(object asset)
        {
            if (!loadedAssetCrcMap.ContainsKey(asset) && !instantiatedAssetMap.ContainsKey(asset))
            {
                throw new InvalidOperationException("You can only release the asset loaded from ResourceManager.");
            }
            
            string bundleName;
            if (loadedAssetCrcMap.ContainsKey(asset))
            {
                bundleName = assetItemMap[loadedAssetCrcMap[asset]].bundleName;
                crcLoadedAssetMap.Remove(loadedAssetCrcMap[asset]);
                loadedAssetCrcMap.Remove(asset);
            }
            else
            {
                bundleName = assetItemMap[loadedAssetCrcMap[instantiatedAssetMap[asset]]].bundleName;
                instantiatedAssetMap.Remove(asset);
                //这里有个问题，移除了实例化的资源，但并没有移除内存中的资源，需要建立一个引用计数
            }
            resourceHelper.UnloadAsset(asset);
            loadedBundleMap[bundleName].refCount--;
            
            if (loadedBundleMap[bundleName].refCount == 0)
            {
                UnloadUnusedBundles();
            }
        }

        public AssetBundleInfo[] GetLoadedBundleData()
        {
            AssetBundleInfo[] result = new AssetBundleInfo[loadedBundleMap.Count];
            int index = 0;
            foreach (var item in loadedBundleMap)
            {
                result[index] = new AssetBundleInfo(item.Key, item.Value.refCount, item.Value.Ready);
                index++;
            }

            return result;
        }

        public AssetItemInfo[] GetLoadedAssetData()
        {
            throw new NotImplementedException();
        }

        private void UnloadUnusedBundles()
        {
            string unusedBundleName = null;
            while (true)
            {
                foreach (BundleItem item in loadedBundleMap.Values)
                {
                    if (item.refCount == 0)
                    {
                        bool inUsed = loadingBundleSet.Contains(item);
                        if (inUsed)
                        {
                            continue;
                        }

                        foreach (BundleItem loadingBundle in loadingBundleSet)
                        {
                            if (loadingBundle.dependencySet.Contains(item.bundleName))
                            {
                                inUsed = true;
                                break;
                            }
                        }
                        if (!inUsed)
                        {
                            unusedBundleName = item.bundleName;
                            break;
                        }
                    }
                }

                if (unusedBundleName == null)
                {
                    break;
                }
                else
                {
                    resourceHelper.UnloadBundle(loadedBundleMap[unusedBundleName].bundleObject);
                    foreach (string bundleDependency in loadedBundleMap[unusedBundleName].dependencySet)
                    {
                        loadedBundleMap[bundleDependency].refCount--;
                    }
                    loadedBundleMap.Remove(unusedBundleName);
                    unusedBundleName = null;
                }
            }
        }

        private string GetBundleFilePath(HQAssetBundleConfig bundleInfo)
        {
            if (bundleFilePathMap.ContainsKey(bundleInfo.bundleName))
            {
                return bundleFilePathMap[bundleInfo.bundleName];
            }
            else
            {
                string bundlePath = resourceHelper.GetBundleFilePath(bundleInfo);
                bundleFilePathMap.Add(bundleInfo.bundleName, bundlePath);
                return bundlePath;
            }
        }

        private void ReloadAssetMap()
        {
            assetItemMap.Clear();
            foreach (var module in localManifest.moduleDic.Values)
            {
                foreach (var asset in module.assetsDic.Values)
                {
                    assetItemMap.Add(asset.crc, asset);
                }
            }
        }
    }
}
