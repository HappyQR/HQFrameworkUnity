using System;
using System.IO;
using HQFramework.Download;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class ResourceComponent : BaseComponent
    {
#if UNITY_EDITOR
        public enum ResourceLoadMode
        {
            Runtime,
            Editor
        }

        [SerializeField]
        private ResourceLoadMode resourceLoadMode;

        [SerializeField]
        private UnityEngine.Object assetRootFolder;
#endif

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
#if UNITY_EDITOR
            if (resourceLoadMode == ResourceLoadMode.Editor)
            {
                string rootDir = UnityEditor.AssetDatabase.GetAssetPath(assetRootFolder);
                resourceManager = new EditorResourceManager(rootDir);
                return;
            }
#endif
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

        public void DecompressBuiltinAssets(Action onComplete)
        {
            resourceManager.DecompressBuiltinAssets(onComplete);
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

        public void LoadAsset(uint crc, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            resourceManager.LoadAsset(crc, onComplete, onError, priority, groupID);
        }

        public void LoadAsset(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0)
        {
            resourceManager.LoadAsset(crc, assetType, onComplete, onError, priority, groupID);
        }

        public void LoadAsset<T>(uint crc, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0) where T : class
        {
            resourceManager.LoadAsset<T>(crc, onComplete, onError, priority, groupID);
        }

        public void LoadAsset(string path, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            resourceManager.LoadAsset(path, onComplete, onError, priority, groupID);
        }

        public void LoadAsset(string path, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0)
        {
            resourceManager.LoadAsset(path, assetType, onComplete, onError, priority, groupID);
        }

        public void LoadAsset<T>(string path, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0) where T : class
        {
            resourceManager.LoadAsset(path, onComplete, onError, priority, groupID);
        }

        public void InstantiateAsset(uint crc, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0)
        {
            resourceManager.InstantiateAsset(crc, onComplete, onError, priority, groupID);
        }

        public void InstantiateAsset(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0)
        {
            resourceManager.InstantiateAsset(crc, assetType, onComplete, onError, priority, groupID);
        }

        public void InstantiateAsset<T>(uint crc, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0) where T : class
        {
            resourceManager.InstantiateAsset<T>(crc, onComplete, onError, priority, groupID);
        }

        public void InstantiateAsset(string path, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0)
        {
            resourceManager.InstantiateAsset(path, onComplete, onError, priority, groupID);
        }

        public void InstantiateAsset(string path, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0)
        {
            resourceManager.InstantiateAsset(path, assetType, onComplete, onError, priority, groupID);
        }

        public void InstantiateAsset<T>(string path, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority = 0, int groupID = 0) where T : class
        {
            resourceManager.InstantiateAsset<T>(path, onComplete, onError, priority, groupID);
        }

        public void ReleaseAsset(object asset)
        {
            resourceManager.ReleaseAsset(asset);
        }

        public AssetBundleInfo[] GetLoadedBundleInfo()
        {
            return resourceManager.GetLoadedBundleInfo();
        }

        public AssetItemInfo[] GetLoadedAssetInfo()
        {
            return resourceManager.GetLoadedAssetInfo();
        }
    }
}
