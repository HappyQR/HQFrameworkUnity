using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class BundleItem
        {
            public string bundleName;
            public int refCount;
            public object bundleObject;
            public HashSet<string> dependencySet;

            public bool Ready => bundleObject != null;

            public BundleItem(AssetBundleInfo targetBundleInfo)
            {
                refCount = 0;
                bundleObject = null;
                bundleName = targetBundleInfo.bundleName;
                dependencySet = new HashSet<string>();
                for (int i = 0; i < targetBundleInfo.dependencies.Length; i++)
                {
                    dependencySet.Add(targetBundleInfo.dependencies[i]);
                }
            }
        }
    }
}
