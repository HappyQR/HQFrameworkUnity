using System;
using System.Collections.Generic;

namespace HQFramework.Editor
{
    public class AssetCompileData
    {
        public Dictionary<AssetModuleConfig, List<AssetBundleCompileInfo>> dataDic = new Dictionary<AssetModuleConfig,List<AssetBundleCompileInfo>>();
    }

    [Serializable]
    public class AssetBundleCompileInfo
    {
        public string filePath;
        public string bundleName;
        public string md5;
        public int size; // unit : byte
        public string[] dependencies;
    }
}
