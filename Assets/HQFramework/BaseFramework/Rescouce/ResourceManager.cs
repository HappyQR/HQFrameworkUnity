using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal sealed partial class ResourceManager : HQModuleBase, IResourceManager
    {
        private IResourceHelper resourceHelper;
        private ResourceLoader resourceLoader;
        private ResourceConfig config;
        private Dictionary<uint, AssetItemInfo> assetItemMap;
        private Dictionary<string, BundleBase> cachedBundleDic;

        public override byte Priority => byte.MaxValue;
        public AssetHotfixMode HotfixMode => config.hotfixMode;
        public string PersistentDir => config.assetPersistentDir;
        public string BuiltinDir => config.assetBuiltinDir;
        public string HotfixUrl => config.hotfixUrl;
        public string HotfixManifestUrl => config.hotfixManifestUrl;

        public static readonly string manifestFileName = "AssetModuleManifest.json";

        protected override void OnInitialize()
        {
            resourceLoader = new ResourceLoader(this);
        }

        public void LoadAsset(uint crc, Type assetType, Action<object> callback)
        {
            if (!assetItemMap.ContainsKey(crc))
            {
                throw new ArgumentException($"Asset(crc : {crc}) doesn't exist.");
            }

            AssetItemInfo item = assetItemMap[crc];
            resourceLoader.LoadAsset(item, assetType, callback);
        }

        public void SetHelper(IResourceHelper resourceHelper)
        {
            this.resourceHelper = resourceHelper;
            config = resourceHelper.LoadResourceConfig();
        }

        public void ReleaseAsset(object asset)
        {
            throw new NotImplementedException();
        }

        public void LoadAsset<T>(uint crc, Action<T> callback) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
