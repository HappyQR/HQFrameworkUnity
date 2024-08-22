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
        public int currentPatchVersion;
        public int minimalSupportedPatchVersion;
        public bool isBuiltin;
        public string releaseNote;
        public Dictionary<string, AssetBundleInfo> bundleDic;
        public string[] dependencies;
        //模块全量资源信息
        public Dictionary<uint, AssetItemInfo> assetsDic;
    }
}
