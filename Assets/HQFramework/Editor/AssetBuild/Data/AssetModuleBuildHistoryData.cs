using System;
using System.Collections.Generic;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    [Serializable]
    public class AssetModuleBuildHistoryData
    {
        //  key: module id, value: module build history list
        public Dictionary<int, List<AssetModuleBuildResult>> moduleBuildData;

        public AssetModuleBuildHistoryData()
        {
            moduleBuildData = new Dictionary<int, List<AssetModuleBuildResult>>();
        }
    }
    
    [Serializable]
    public class AssetModuleBuildResult
    {
        public int moduleID;
        public string moduleName;
        public uint buildVersionCode;
        public string devNotes;
        public string buildTime;
        public Dictionary<string, AssetBundleInfo> bundlePathMap;
        public Dictionary<uint, AssetItemInfo> assetsDic;
        public int[] dependencies;
    }
}
