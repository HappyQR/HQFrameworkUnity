using System;
using System.Collections.Generic;
using System.IO;

namespace HQFramework.Resource
{
    internal sealed partial class ResourceManager : HQModuleBase, IResourceManager
    {
        private ResourceConfig config;
        private IResourceHelper resourceHelper;
        private ResourceLoader resourceLoader;
        private ResourceHotfixChecker hotfixChecker;
        private ResourceDownloader resourceDownloader;

        private AssetModuleManifest localManifest;
        private AssetModuleManifest remoteManifest;

        private Dictionary<AssetModuleInfo, List<AssetBundleInfo>> necessaryHotfixContent;
        private Dictionary<AssetModuleInfo, List<AssetBundleInfo>> separateHotfixContent;
        
        private Action<HotfixCheckErrorEventArgs> onHotfixCheckError;
        private Action<HotfixCheckCompleteEventArgs> onHotfixCheckComplete;
        private Action<HotfixDownloadUpdateEventArgs> onHotfixDownloadUpdate;
        private Action<HotfixDownloadErrorEventArgs> onHotfixDownloadError;
        private Action<HotfixDownloadCompleteEventArgs> onHotfixDownloadComplete;

        public override byte Priority => byte.MaxValue;
        public AssetHotfixMode HotfixMode => config.hotfixMode;
        public string PersistentDir => config.assetPersistentDir;
        public string BuiltinDir => config.assetBuiltinDir;
        public string HotfixUrl => config.hotfixUrl;
        public string HotfixManifestUrl => config.hotfixManifestUrl;


        public event Action<HotfixCheckErrorEventArgs> HotfixCheckErrorEvent
        {
            add { onHotfixCheckError += value; }
            remove { onHotfixCheckError -= value; }
        }
        public event Action<HotfixCheckCompleteEventArgs> HotfixCheckCompleteEvent
        {
            add { onHotfixCheckComplete += value; }
            remove { onHotfixCheckComplete -= value; }
        }
        public event Action<HotfixDownloadUpdateEventArgs> HotfixDownloadUpdateEvent
        {
            add { onHotfixDownloadUpdate += value; }
            remove { onHotfixDownloadUpdate -= value; }
        }
        public event Action<HotfixDownloadErrorEventArgs> HotfixDownloadErrorEvent
        {
            add { onHotfixDownloadError += value; }
            remove { onHotfixDownloadError -= value; }
        }
        public event Action<HotfixDownloadCompleteEventArgs> HotfixDownloadCompleteEvent
        {
            add { onHotfixDownloadComplete += value; }
            remove { onHotfixDownloadComplete -= value;}
        }

        protected override void OnInitialize()
        {
            
        }

        protected override void OnUpdate()
        {
            resourceDownloader?.OnUpdate();
        }

        public void SetHelper(IResourceHelper resourceHelper)
        {
            this.resourceHelper = resourceHelper;
            config = resourceHelper.LoadResourceConfig();
        }

        public async void CheckHotfix()
        {
            if (config.hotfixMode == AssetHotfixMode.NoHotfix)
            {
                throw new InvalidOperationException("You can't use CheckHotfix under NoHotfix mode.");
            }
            if (hotfixChecker == null)
            {
                hotfixChecker = new ResourceHotfixChecker(this);
            }
            if (localManifest == null)
            {
                localManifest = await resourceHelper.LoadAssetManifestAsync();
            }
            hotfixChecker.CheckHotfix();
        }

        public void StartHotfix()
        {
            if (config.hotfixMode == AssetHotfixMode.NoHotfix)
            {
                throw new InvalidOperationException("You can't use CheckHotfix under NoHotfix mode.");
            }
            if (necessaryHotfixContent == null || necessaryHotfixContent.Count == 0)
            {
                throw new InvalidOperationException("Nothing to update.");
            }
            if (resourceDownloader == null)
            {
                resourceDownloader = new ResourceDownloader(this);
            }
            resourceDownloader.StartHotfix();
        }

        public void LoadAsset(uint crc, Type assetType, Action<object> callback)
        {
            throw new NotImplementedException();
        }

        public void ReleaseAsset(object asset)
        {
            throw new NotImplementedException();
        }

        public void LoadAsset<T>(uint crc, Action<T> callback) where T : class
        {
            throw new NotImplementedException();
        }

        public HotfixCheckCompleteEventArgs CheckModuleHotfix(int moduleID)
        {
            if (config.hotfixMode != AssetHotfixMode.SeparateHotfix)
            {
                throw new InvalidOperationException("CheckModuleHotfix() only adapt to SeparateHotfix mode.");
            }

            return hotfixChecker.CheckModuleHotfix(moduleID);
        }

        public int StartModuleHotfix(int moduleID)
        {
            if (config.hotfixMode != AssetHotfixMode.SeparateHotfix)
            {
                throw new InvalidOperationException("CheckModuleHotfix() only adapt to SeparateHotfix mode.");
            }

            AssetModuleInfo remoteModule = remoteManifest.moduleDic[moduleID];
            if (separateHotfixContent.ContainsKey(remoteModule))
            {
                return resourceDownloader.DownloadModule(remoteModule, separateHotfixContent[remoteModule]);
            }
            else
            {
                throw new InvalidOperationException("Nothing to update");
            }
        }
    }
}
