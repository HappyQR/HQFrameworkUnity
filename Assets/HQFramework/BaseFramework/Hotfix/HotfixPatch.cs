using System.Collections.Generic;
using HQFramework.Resource;

namespace HQFramework.Hotfix
{
    internal class HotfixPatch
    {
        public readonly AssetModuleInfo module;

        public readonly List<AssetBundleInfo> bundleList;

        public HotfixPatch(AssetModuleInfo module, IEnumerable<AssetBundleInfo> bundleList)
        {
            this.module = module;
            this.bundleList = new List<AssetBundleInfo>(bundleList);
        }
    }
}
