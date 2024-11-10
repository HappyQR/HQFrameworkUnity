using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class AssetItem
        {
            public uint crc;
            public int refCount;
            public object assetObject;
            public bool error;
            public HashSet<uint> dependencies;

            public bool Ready => assetObject != null;

            public AssetItem(HQAssetItemConfig itemConfig, object assetObject)
            {
                this.crc = itemConfig.crc;
                this.refCount = 0;
                this.assetObject = assetObject;
                this.error = false;
                this.dependencies = new HashSet<uint>();
                for (int i = 0; i < itemConfig.dependencies.Length; i++)
                {
                    dependencies.Add(itemConfig.dependencies[i]);
                }
            }
        }
    }
}
