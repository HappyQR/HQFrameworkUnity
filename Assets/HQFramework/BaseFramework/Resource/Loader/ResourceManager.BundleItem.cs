using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class BundleItem
        {
            public uint crc;
            public int refCount;
            public object bundleObject;
            public HashSet<uint> dependencySet;
            public ResourceStatus status;

            public BundleItem(HQAssetBundleConfig bundleConfig)
            {
                this.crc = bundleConfig.crc;
                this.status = ResourceStatus.Pending;
                this.refCount = 0;
                this.bundleObject = null;
                this.dependencySet = new HashSet<uint>();
                for (int i = 0; i < bundleConfig.dependencies.Length; i++)
                {
                    dependencySet.Add(bundleConfig.dependencies[i]);
                }
            }
        }
    }
}
