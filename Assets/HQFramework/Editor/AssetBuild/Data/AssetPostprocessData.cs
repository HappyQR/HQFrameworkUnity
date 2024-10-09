using System;
using System.Collections.Generic;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    public class AssetPostprocessData
    {
        public List<AssetModuleCompileInfo> dataList = new List<AssetModuleCompileInfo>();
    }

    [Serializable]
    public class AssetModuleCompileInfo
    {
        public int moduleID;
        public string moduleName;
        public uint buildVersionCode;
        public string devNotes;
        public string buildTime;
        public List<AssetBundleCompileInfo> bundleList;
        public Dictionary<uint, AssetItemInfo> assetsDic;
        public int[] dependencies;
    }
}
