using System.Collections.Generic;

namespace HQFramework.Editor
{
    public class AssetPreprocessResult
    {
        public Dictionary<AssetModuleConfig, AssetBundleBuildInfo> moduleBundleBuildsDic;
    }

    public class AssetBundleBuildInfo
    {
        public string bundleName;
        public string[] bundleAssets;
    }
}
