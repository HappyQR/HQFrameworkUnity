using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    [Serializable]
    public class AssetModuleManifest
    {
        public string productName;
        public string productVersion;
        public string runtimePlatform;
        public int resourceVersion;
        public int minimalSupportedVersion;
        public string releaseNote;
        public bool isBuiltinManifest;
        public Dictionary<int, AssetModuleInfo> moduleDic;
    }
}