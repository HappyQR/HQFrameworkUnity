using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class AssetPendingItem : IReference
        {
            public uint crc
            {
                get;
                private set;
            }

            public HashSet<uint> dependenceSet
            {
                get;
                private set;
            }

            public int count
            {
                get;
                set;
            }

            public static AssetPendingItem Create(HQAssetItemConfig assetConfig)
            {
                AssetPendingItem item = ReferencePool.Spawn<AssetPendingItem>();
                item.crc = assetConfig.crc;
                if (item.dependenceSet == null)
                {
                    item.dependenceSet = new HashSet<uint>();
                }
                for (int i = 0; i < assetConfig.dependencies.Length; i++)
                {
                    item.dependenceSet.Add(assetConfig.dependencies[i]);
                }
                return item;
            }

            void IReference.OnRecyle()
            {
                crc = 0;
                count = 0;
                dependenceSet.Clear();
            }
        }
    }
}
