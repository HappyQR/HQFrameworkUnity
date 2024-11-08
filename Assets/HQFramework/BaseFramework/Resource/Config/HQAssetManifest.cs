using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    [Serializable]
    public class HQAssetManifest
    {
        public string productName;
        public string productVersion;
        public string resourceVersion;
        public int versionCode;
        public int minimalSupportedVersionCode;
        public string releaseNote;
        public bool isBuiltinManifest;
        public Dictionary<int, HQAssetModuleConfig> moduleDic;
    }
}