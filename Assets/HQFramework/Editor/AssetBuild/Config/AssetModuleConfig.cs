using System.Collections.Generic;
using System;

namespace HQFramework.Editor
{
    [Serializable]
    public class AssetModuleConfig
    {
        public int id;
        public string moduleName;
        public string createTime;
        public bool isBuiltin;
        public int buildVersionCode;
        public string devNotes;
        public List<AssetBundleConfig> bundleConfigList;
    }

    [Serializable]
    public class AssetBundleConfig
    {
        public string bundleName;
        public List<string> assetItemList;
    }
}
