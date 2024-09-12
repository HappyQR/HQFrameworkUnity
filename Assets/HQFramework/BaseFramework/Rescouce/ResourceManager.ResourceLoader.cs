using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class ResourceLoader
        {
            private ResourceManager resourceManager;

            public ResourceLoader(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
            }

            public void LoadAsset(AssetItemInfo assetInfo, Type assetType, Action<object> callback)
            {
                
            }

            public void LoadAsset<T>(AssetItemInfo assetInfo, Action<T> callback) where T : class
            {
                
            }

            public Dictionary<uint, AssetItemInfo> LoadAssetsMap()
            {
                throw new NotImplementedException();
            }
        }
    }
}
