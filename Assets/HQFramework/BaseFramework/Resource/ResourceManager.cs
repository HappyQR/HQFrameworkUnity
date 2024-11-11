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

        // data table
        private Dictionary<uint, HQAssetBundleConfig> bundleTable;
        private Dictionary<uint, HQAssetItemConfig> assetTable;

        // object map
        private Dictionary<uint, string> bundleFilePathMap;
        private Dictionary<uint, BundleItem> loadedBundleMap;
        private Dictionary<uint, AssetItem> loadedAssetMap;

        private bool isAssetsDecompressed = false;
        private Queue<ResourceLoadTaskInfo> loadTaskWaitingQueue;

        public override byte Priority => byte.MaxValue;
        public string PersistentDir => resourceHelper.AssetsPersistentDir;
        public string BuiltinDir => resourceHelper.AssetsBuiltinDir;

        private static readonly ushort maxConcurrentLoadCount = 1024;

        protected override void OnInitialize()
        {
            bundleTable = new Dictionary<uint, HQAssetBundleConfig>();
            assetTable = new Dictionary<uint, HQAssetItemConfig>();

            bundleFilePathMap = new Dictionary<uint, string>();
            loadedBundleMap = new Dictionary<uint, BundleItem>();
            loadedAssetMap = new Dictionary<uint, AssetItem>();

            loadTaskWaitingQueue = new Queue<ResourceLoadTaskInfo>();
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

            void OnManifestLoadComplete(ManifestLoadCompleteEventArgs args)
            {
                localManifest = args.manifest;
                ReloadAssetTable();
                while (loadTaskWaitingQueue.Count > 0)
                {
                    ResourceLoadTaskInfo taskInfo = loadTaskWaitingQueue.Dequeue();
                    resourceLoader.LoadAsset(taskInfo);
                }
            }

            resourceHelper.LoadAssetManifest(OnManifestLoadComplete);
        }

        public void SetDownloadManager(IDownloadManager downloadManager)
        {
            this.downloadManager = downloadManager;
            resourceDownloader = new ResourceDownloader(this);
        }

        public void DecompressBuiltinAssets(Action onComplete)
        {
            void OnDecompressComplete()
            {
                isAssetsDecompressed = true;
                onComplete?.Invoke();
            }
            resourceHelper.DecompressBuiltinAssets(OnDecompressComplete);
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
            if (localManifest == null)
            {
                throw new InvalidOperationException("The Resource Module has not been initialized yet.");
            }
            return localManifest.moduleDic.ContainsKey(moduleID);
        }

        public void LoadAsset(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            ResourceLoadTaskInfo taskInfo = ResourceLoadTaskInfo.Create(crc, assetType, onComplete, onError, priority, groupID);
            if (localManifest == null)
            {
                loadTaskWaitingQueue.Enqueue(taskInfo);
            }
            else
            {
                resourceLoader.LoadAsset(taskInfo);
            }
        }

        public void LoadAsset(uint crc, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            LoadAsset(crc, null, onComplete, onError, priority, groupID);
        }

        public void LoadAsset<T>(uint crc, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class
        {
            void OnLoadComplete(ResourceLoadCompleteEventArgs originalArgs)
            {
                ResourceLoadCompleteEventArgs<T> args = ResourceLoadCompleteEventArgs<T>.Create(originalArgs.crc, (T)originalArgs.asset);
                onComplete?.Invoke(args);
                ReferencePool.Recyle(args);
            }

            Type assetType = typeof(T);
            LoadAsset(crc, assetType, OnLoadComplete, onError, priority, groupID);
        }

        public void LoadAsset(string path, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            uint crc = Utility.CRC32.ComputeCrc32(path);
            LoadAsset(crc, assetType, onComplete, onError, priority, groupID);
        }

        public void LoadAsset(string path, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            uint crc = Utility.CRC32.ComputeCrc32(path);
            LoadAsset(crc, null, onComplete, onError, priority, groupID);
        }

        public void LoadAsset<T>(string path, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class
        {
            uint crc = Utility.CRC32.ComputeCrc32(path);
            LoadAsset<T>(crc, onComplete, onError, priority, groupID);
        }

        public object InstantiateAsset(object asset)
        {
            throw new NotImplementedException();
        }

        public T InstantiateAsset<T>(T asset) where T : class
        {
            throw new NotImplementedException();
        }

        public void ReleaseAsset(object asset)
        {
            throw new NotImplementedException();
        }

        public AssetBundleInfo[] GetLoadedBundleData()
        {
            throw new NotImplementedException();
        }

        public AssetItemInfo[] GetLoadedAssetData()
        {
            throw new NotImplementedException();
        }

        private string GetBundleFilePath(HQAssetBundleConfig bundleInfo)
        {
            if (bundleFilePathMap.ContainsKey(bundleInfo.crc))
            {
                return bundleFilePathMap[bundleInfo.crc];
            }
            else
            {
                string bundleRootDir = isAssetsDecompressed ? PersistentDir : BuiltinDir;
                string bundlePath = Path.Combine(bundleRootDir, resourceHelper.GetBundleRelatedPath(bundleInfo));
                bundleFilePathMap.Add(bundleInfo.crc, bundlePath);
                return bundlePath;
            }
        }

        private void ReloadAssetTable()
        {
            bundleTable.Clear();
            assetTable.Clear();

            foreach (HQAssetModuleConfig module in localManifest.moduleDic.Values)
            {
                foreach (HQAssetBundleConfig bundle in module.bundleDic.Values)
                {
                    bundleTable.Add(bundle.crc, bundle);
                }

                foreach (HQAssetItemConfig asset in module.assetsDic.Values)
                {
                    assetTable.Add(asset.crc, asset);
                }
            }
        }
    }
}
