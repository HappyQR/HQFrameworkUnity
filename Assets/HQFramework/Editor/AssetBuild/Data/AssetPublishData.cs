using System;
using System.Collections.Generic;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    [Serializable]
    public class AssetPublishData
    {
        public string resourceVersion;
        public int versionCode;
        public int minimalSupportedVersionCode;
        public string releaseNote;
        public Dictionary<int, HQAssetModuleConfig> moduleDic;
        public Dictionary<string, string> bundleFileMap;
    }

    [Serializable]
    public class AssetBundleUploadItem
    {
        public HQAssetModuleConfig moduleInfo;
        public HQAssetBundleConfig bundleInfo;
        public string bundleFilePath;
    }
}
