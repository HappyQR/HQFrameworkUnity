using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal sealed partial class ResourceManager : HQModuleBase, IResourceManager
    {
        private IResourceHelper resourceHelper;
        private ResourceConfig config;
        private Dictionary<uint, AssetItemInfo> assetItemMap;
        private Dictionary<string, AssetBundleItem> cachedBundleDic;



        public override byte Priority => byte.MaxValue;
        public ResourceConfig Config => config;

        public static readonly string manifestFileName = "AssetModuleManifest.json";

        protected override void OnInitialize()
        {
            
        }

        public void SetHelper(IResourceHelper resourceHelper)
        {
            this.resourceHelper = resourceHelper;
            config = resourceHelper.LoadResourceConfig();
        }

        public void LoadAsset(uint crc)
        {
            if (!assetItemMap.ContainsKey(crc))
            {
                throw new ArgumentException($"Asset(crc : {crc}) doesn't exist.");
            }

            AssetItemInfo item = assetItemMap[crc];
            if (cachedBundleDic.ContainsKey(item.bundleName))
            {
                cachedBundleDic[item.bundleName].referenceCount++;
            }
        }
    }
}
