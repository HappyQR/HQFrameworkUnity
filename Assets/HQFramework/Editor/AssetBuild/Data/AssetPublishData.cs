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
        public Dictionary<int, AssetModuleInfo> moduleDic;
        public Dictionary<string, string> bundleFileMap;
    }

    [Serializable]
    public class AssetBundleUploadItem
    {
        public AssetModuleInfo moduleInfo;
        public AssetBundleInfo bundleInfo;
        public string bundleFilePath;
    }
}
