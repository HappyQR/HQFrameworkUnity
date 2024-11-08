using System;
using System.IO;
using HQFramework.Download;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class ResourceComponent : BaseComponent
    {
        [SerializeField]
        private string resourceHelperTypeName;
        [SerializeField]
        private HQHotfixMode hotfixMode;
        [SerializeField]
        private int launcherHotfixID;
        [SerializeField]
        private string assetsPersistentDir;
        [SerializeField]
        private string assetsBuiltinDir;
        [SerializeField]
        private string hotfixManifestUrl;

        private IResourceManager resourceManager;

        public string PersistentDir => resourceManager.PersistentDir;
        public string BuiltinDir => resourceManager.BuiltinDir;

        private void Start()
        {
            InitializeResourceHelper();
            IDownloadManager downloadManager = HQFrameworkEngine.GetModule<IDownloadManager>();
            resourceManager.SetDownloadManager(downloadManager);
        }

        private void InitializeResourceHelper()
        {
            Type helperType = Utility.Assembly.GetType(resourceHelperTypeName);
            IResourceHelper resourceHelper = Activator.CreateInstance(helperType) as IResourceHelper;
            resourceHelper.HotfixMode = hotfixMode;
            resourceHelper.LauncherHotfixID = launcherHotfixID;
            resourceHelper.AssetsPersistentDir = Path.Combine(Application.persistentDataPath, assetsPersistentDir);
            resourceHelper.AssetsBuiltinDir = Path.Combine(Application.streamingAssetsPath, assetsBuiltinDir);
            resourceHelper.HotfixManifestUrl = hotfixManifestUrl;

            resourceManager = HQFrameworkEngine.GetModule<IResourceManager>();
            resourceManager.SetHelper(resourceHelper);
        }

        public int LaunchHotfixCheck()
        {
            return resourceManager.LaunchHotfixCheck();
        }

        public int LaunchHotfix()
        {
            return resourceManager.LaunchHotfix();
        }

        public int ModuleHotfixCheck(int moduleID)
        {
            return resourceManager.ModuleHotfixCheck(moduleID);
        }

        public int ModuleHotfix(int moduleID)
        {
            return resourceManager.ModuleHotfix(moduleID);
        }

        public void PauseHotfix(int hotfixID)
        {
            resourceManager.PauseHotfix(hotfixID);
        }

        public void ResumeHotfix(int hotfixID)
        {
            resourceManager.ResumeHotfix(hotfixID);
        }

        public void CancelHotfix(int hotfixID)
        {
            resourceManager.CancelHotfix(hotfixID);
        }

        public void AddHotfixCheckErrorEvent(int hotfixID, Action<HotfixCheckErrorEventArgs> onHotfixCheckError)
        {
            resourceManager.AddHotfixCheckErrorEvent(hotfixID, onHotfixCheckError);
        }

        public void AddHotfixCheckCompleteEvent(int hotfixID, Action<HotfixCheckCompleteEventArgs> onHotfixCheckComplete)
        {
            resourceManager.AddHotfixCheckCompleteEvent(hotfixID, onHotfixCheckComplete);
        }

        public void AddHotfixDownloadUpdateEvent(int hotfixID, Action<HotfixDownloadUpdateEventArgs> onHotfixUpdate)
        {
            resourceManager.AddHotfixDownloadUpdateEvent(hotfixID, onHotfixUpdate);
        }

        public void AddHotfixDownloadErrorEvent(int hotfixID, Action<HotfixDownloadErrorEventArgs> onHotfixError)
        {
            resourceManager.AddHotfixDownloadErrorEvent(hotfixID, onHotfixError);
        }

        public void AddHotfixDownloadPauseEvent(int hotfixID, Action<HotfixDownloadPauseEventArgs> onHotfixPause)
        {
            resourceManager.AddHotfixDownloadPauseEvent(hotfixID, onHotfixPause);
        }

        public void AddHotfixDownloadResumeEvent(int hotfixID, Action<HotfixDownloadResumeEventArgs> onHotfixResume)
        {
            resourceManager.AddHotfixDownloadResumeEvent(hotfixID, onHotfixResume);
        }

        public void AddHotfixDownloadCancelEvent(int hotfixID, Action<HotfixDownloadCancelEventArgs> onHotfixCancel)
        {
            resourceManager.AddHotfixDownloadCancelEvent(hotfixID, onHotfixCancel);
        }

        public void AddHotfixDownloadCompleteEvent(int hotfixID, Action<HotfixDownloadCompleteEventArgs> onHotfixComplete)
        {
            resourceManager.AddHotfixDownloadCompleteEvent(hotfixID, onHotfixComplete);
        }

        public bool HasModule(int moduleID)
        {
            return resourceManager.HasModule(moduleID);
        }

        public void LoadAsset(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0)
        {
            resourceManager.LoadAsset(crc, assetType, onComplete, onError, priority, groupID);
        }

        public void LoadAsset<T>(uint crc, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0) where T : class
        {
            resourceManager.LoadAsset<T>(crc, onComplete, onError, priority, groupID);
        }

        public void LoadAsset(string path, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0)
        {
            resourceManager.LoadAsset(path, assetType, onComplete, onError, priority, groupID);
        }

        public void LoadAsset<T>(string path, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0) where T : class
        {
            resourceManager.LoadAsset(path, onComplete, onError, priority, groupID);
        }

        public AssetBundleInfo[] GetLoadedBundleData()
        {
            return resourceManager.GetLoadedBundleData();
        }

        public void ReleaseAsset(object asset)
        {
            resourceManager.ReleaseAsset(asset);
        }

        public object InstantiateAsset(object asset)
        {
            return resourceManager.InstantiateAsset(asset);
        }

        public T InstantiateAsset<T>(T asset) where T : class
        {
            return resourceManager.InstantiateAsset<T>(asset);
        }

        public AssetItemInfo[] GetLoadedAssetData()
        {
            return resourceManager.GetLoadedAssetData();
        }
    }
}
