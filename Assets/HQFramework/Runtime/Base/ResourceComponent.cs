using System;
using System.IO;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class ResourceComponent : BaseComponent
    {
        public string resourceHelperTypeName;
        public AssetHotfixMode hotfixMode;
        public int launcherHotfixID;
        public string assetsPersistentDir;
        public string assetsBuiltinDir;
        public string hotfixManifestUrl;

        private IResourceManager resourceManager;

        private void Start()
        {
            InitializeResourceHelper();
        }

        private void InitializeResourceHelper()
        {
            Type helperType = Type.GetType(resourceHelperTypeName);
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

        public void LoadAsset(uint crc, Type assetType, Action<object> callback)
        {
            throw new NotImplementedException();
        }

        public void LoadAsset<T>(uint crc, Action<T> callback) where T : class
        {
            throw new NotImplementedException();
        }

        public void ReleaseAsset(object asset)
        {
            throw new NotImplementedException();
        }
    }
}
