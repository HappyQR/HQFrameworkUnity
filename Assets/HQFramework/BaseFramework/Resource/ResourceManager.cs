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

        private AssetModuleManifest localManifest;
        private AssetModuleManifest remoteManifest;

        private Dictionary<AssetModuleInfo, List<AssetBundleInfo>> necessaryHotfixContent;
        private Dictionary<AssetModuleInfo, List<AssetBundleInfo>> separateHotfixContent;
        private Dictionary<string, string> bundleFilePathDic;

        public override byte Priority => byte.MaxValue;
        public string PersistentDir => resourceHelper.AssetsPersistentDir;
        public string BuiltinDir => resourceHelper.AssetsBuiltinDir;

        protected override void OnInitialize()
        {
            bundleFilePathDic = new Dictionary<string, string>();
        }

        protected override void OnUpdate()
        {
            resourceDownloader?.OnUpdate();
        }

        public void SetHelper(IResourceHelper resourceHelper)
        {
            this.resourceHelper = resourceHelper;
            hotfixChecker = new ResourceHotfixChecker(this);
            resourceDownloader = new ResourceDownloader(this);
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

        private string GetBundleFilePath(AssetBundleInfo bundleInfo)
        {
            if (bundleFilePathDic.ContainsKey(bundleInfo.bundleName))
            {
                return bundleFilePathDic[bundleInfo.bundleName];
            }
            else
            {
                string bundlePath = resourceHelper.GetBundleFilePath(bundleInfo);
                bundleFilePathDic.Add(bundleInfo.bundleName, bundlePath);
                return bundlePath;
            }
        }
    }
}
