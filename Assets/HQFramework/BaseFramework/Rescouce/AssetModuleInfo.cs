using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    [Serializable]
    public class AssetModuleInfo
    {
        public int id;
        public string moduleName;
        public string description;
        // 全量版本号
        public int rootVersion;
        // 增量补丁号
        public int currentPatchVersion;
        // 最小支持补丁号
        public int minimalSupportedPatchVersion;
        public bool isBuiltin;
        public string releaseNote;
        public Dictionary<string, AssetBundleInfo> bundleDic;
        public string[] dependencies;
        //模块全量资源信息
        public Dictionary<uint, AssetItemInfo> assetsDic;
    }
}
