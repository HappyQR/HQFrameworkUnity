using System.Collections.Generic;

namespace HQFramework.Editor
{
    public class AssetCompileResult
    {
        public Dictionary<AssetModuleConfig, AssetBundleBuildResult[]> moduleBundleDic;

        public AssetCompileResult()
        {
            moduleBundleDic = new Dictionary<AssetModuleConfig, AssetBundleBuildResult[]>();
        }
    }

    public class AssetBundleBuildResult
    {
        public string filePath;
        public string bundleName;
        public string md5;
        public int size; // unit : byte
        public string[] dependencies;
    }
}
