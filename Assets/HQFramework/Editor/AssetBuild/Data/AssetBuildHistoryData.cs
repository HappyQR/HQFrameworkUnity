using System;
using System.Collections.Generic;

namespace HQFramework.Editor
{
    [Serializable]
    public class AssetBuildHistoryData
    {
        //  key: module id, value: module build history list
        public Dictionary<int, List<AssetModuleBuildInfo>> moduleBuildData;
    }
}
