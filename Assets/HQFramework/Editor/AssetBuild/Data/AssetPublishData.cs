using System;
using System.Collections.Generic;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    [Serializable]
    public class AssetPublishData
    {
        public string tag;
        public bool uploaded;
        public int resourceVersion;
        public int minimalSupportedVersion;
        public string releaseNote;
        public Dictionary<int, AssetModuleInfo> moduleDic;
    }
}
