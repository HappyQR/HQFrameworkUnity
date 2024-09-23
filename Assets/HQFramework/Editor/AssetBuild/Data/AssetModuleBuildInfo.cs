using System;
using System.Collections.Generic;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    [Serializable]
    public class AssetModuleBuildInfo
    {
        public int id;
        public string moduleName;
        public string dir;
        public int buildVersionCode;
        public string devNotes;
        public long buildTimeTicks;
        public DateTime buildTime;
        public Dictionary<uint, AssetItemInfo> assetsDic;
        public Dictionary<AssetBundleInfo, string> bundlePathMap;
    }
}
