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
            public ResourceStatus status;
            public HashSet<uint> dependencies;

            public AssetItem(HQAssetItemConfig itemConfig)
            {
                this.crc = itemConfig.crc;
                this.status = ResourceStatus.Pending;
                this.refCount = 0;
                this.assetObject = null;
                this.dependencies = new HashSet<uint>();
                for (int i = 0; i < itemConfig.dependencies.Length; i++)
                {
                    dependencies.Add(itemConfig.dependencies[i]);
                }
            }
        }
    }
}
