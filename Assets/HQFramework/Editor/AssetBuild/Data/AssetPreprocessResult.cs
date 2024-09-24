using System.Collections.Generic;

namespace HQFramework.Editor
{
    public class AssetPreprocessResult
    {
        public Dictionary<AssetModuleConfig, List<AssetBundleBuildInfo>> moduleBundleBuildsDic;

        public AssetPreprocessResult()
        {
            moduleBundleBuildsDic = new Dictionary<AssetModuleConfig, List<AssetBundleBuildInfo>>();
        }
    }

    public class AssetBundleBuildInfo
    {
        public string bundleName;
        public string[] bundleAssets;
    }
}
