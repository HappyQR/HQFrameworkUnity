using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    [Serializable]
    public class AssetModuleManifest
    {
        public int genericVersion;
        public string releaseNote;
        public Dictionary<int, AssetModuleInfo> moduleDic;
    }
}