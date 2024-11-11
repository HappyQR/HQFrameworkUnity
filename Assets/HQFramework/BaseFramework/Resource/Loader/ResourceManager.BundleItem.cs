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
            public bool error;
            public HashSet<uint> dependencySet;

            public bool Ready => bundleObject != null;

            public BundleItem(HQAssetBundleConfig bundleConfig, object bundleObject)
            {
                this.crc = bundleConfig.crc;
                this.refCount = 0;
                this.bundleObject = bundleObject;
                this.dependencySet = new HashSet<uint>();
                for (int i = 0; i < bundleConfig.dependencies.Length; i++)
                {
                    dependencySet.Add(bundleConfig.dependencies[i]);
                }
            }
        }
    }
}
