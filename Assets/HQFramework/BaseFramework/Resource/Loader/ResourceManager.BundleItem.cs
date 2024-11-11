using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class BundleItem
        {
            private string bundleName;
            public int refCount;
            public object bundleObject;
            public bool error;
            public HashSet<string> dependencySet;

            public bool Ready => bundleObject != null;

            public BundleItem(HQAssetBundleConfig bundleConfig, object bundleObject)
            {
                this.bundleName = bundleConfig.bundleName;
                this.refCount = 0;
                this.bundleObject = bundleObject;
                this.dependencySet = new HashSet<string>();
                for (int i = 0; i < bundleConfig.dependencies.Length; i++)
                {
                    dependencySet.Add(bundleConfig.dependencies[i]);
                }
            }
        }
    }
}
