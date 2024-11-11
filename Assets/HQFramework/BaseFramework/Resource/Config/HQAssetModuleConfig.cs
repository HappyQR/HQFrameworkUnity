using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    [Serializable]
    public class HQAssetModuleConfig
    {
        public int id;
        public string moduleName;
        public string description;
        public int currentPatchVersion;
        public int minimalSupportedPatchVersion;
        public bool isBuiltin;
        public string moduleUrlRoot;
        public string releaseNote;
        public int[] dependencies;
        public Dictionary<uint, HQAssetBundleConfig> bundleDic;
        public Dictionary<uint, HQAssetItemConfig> assetsDic;
    }
}
