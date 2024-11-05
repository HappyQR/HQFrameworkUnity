using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal sealed partial class ResourceManager : HQModuleBase, IResourceManager
    {
        private IResourceHelper resourceHelper;
        private ResourceHotfixChecker hotfixChecker;
        private ResourceDownloader resourceDownloader;
        private ResourceLoader resourceLoader;
        private BundleLoader bundleLoader;

        private AssetModuleManifest localManifest;
        private AssetModuleManifest remoteManifest;
        private Dictionary<AssetModuleInfo, List<AssetBundleInfo>> necessaryHotfixContent;
        private Dictionary<AssetModuleInfo, List<AssetBundleInfo>> separateHotfixContent;

        private Dictionary<string, string> bundleFilePathMap;
        private Dictionary<uint, AssetItemInfo> assetItemMap;
        private Dictionary<string, BundleItem> loadedBundleMap;
        private Dictionary<object, string> loadedAssetMap; // key: asset object, value: bundle name

        public override byte Priority => byte.MaxValue;
        public string PersistentDir => resourceHelper.AssetsPersistentDir;
        public string BuiltinDir => resourceHelper.AssetsBuiltinDir;

        private static readonly ushort maxConcurrentLoadCount = 1024;

        protected override void OnInitialize()
        {
            bundleFilePathMap = new Dictionary<string, string>();
            assetItemMap = new Dictionary<uint, AssetItemInfo>();
            loadedBundleMap = new Dictionary<string, BundleItem>();
            loadedAssetMap = new Dictionary<object, string>();
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
            resourceDownloader = new ResourceDownloader(this);
            resourceLoader = new ResourceLoader(this);
            bundleLoader = new BundleLoader(this);

            localManifest = resourceHelper.LoadAssetManifest();
            ReloadAssetMap();
        }

        public int LaunchHotfixCheck()
        {
            if (resourceHelper.HotfixMode == AssetHotfixMode.NoHotfix)
            {
                throw new InvalidOperationException("You can't use CheckHotfix under NoHotfix mode.");
            }

            return hotfixChecker.LaunchHotfix();
        }

        public int ModuleHotfixCheck(int moduleID)
        {
            if (resourceHelper.HotfixMode != AssetHotfixMode.SeparateHotfix)
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
            if (resourceHelper.HotfixMode == AssetHotfixMode.NoHotfix)
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
            if (resourceHelper.HotfixMode != AssetHotfixMode.SeparateHotfix)
            {
                throw new InvalidOperationException("CheckModuleHotfix() only adapt to SeparateHotfix mode.");
            }

            AssetModuleInfo remoteModule = remoteManifest.moduleDic[moduleID];
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

        public void ReleaseAsset(object asset)
        {
            if (!loadedAssetMap.ContainsKey(asset))
            {
                throw new InvalidOperationException("You can only release the asset loaded from ResourceManager.");
            }
            string bundleName = loadedAssetMap[asset];
            loadedBundleMap[bundleName].refCount--;
        }

        public BundleData[] GetLoadedBundleData()
        {
            BundleData[] result = new BundleData[loadedBundleMap.Count];
            int index = 0;
            foreach (var item in loadedBundleMap)
            {
                result[index] = new BundleData(item.Key, item.Value.refCount, item.Value.Ready);
                index++;
            }

            return result;
        }

        public AssetData[] GetLoadedAssetData()
        {
            throw new NotImplementedException();
        }

        private string GetBundleFilePath(AssetBundleInfo bundleInfo)
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
