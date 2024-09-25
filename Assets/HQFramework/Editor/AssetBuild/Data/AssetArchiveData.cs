using System;
using System.Collections.Generic;

namespace HQFramework.Editor
{
    [Serializable]
    public class AssetArchiveData
    {
        public List<AssetArchiveInfo> assetArchiveList;

        public AssetArchiveData()
        {
            assetArchiveList = new List<AssetArchiveInfo>();
        }
    }

    [Serializable]
    public class AssetArchiveInfo
    {
        public int archiveVersionCode;
        public string archiveTime;
        public string archiveNotes;
        public Dictionary<int, AssetModuleBuildResult> moduleBuildResultDic;
    }
}
