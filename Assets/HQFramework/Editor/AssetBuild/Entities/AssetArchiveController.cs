using System;
using System.Collections.Generic;

namespace HQFramework.Editor
{
    public sealed class AssetArchiveController
    {
        public static AssetArchiveData ArchiveAssetModules(List<AssetModuleCompileInfo> moduleCompileInfoList, string archiveTag, string archiveNotes)
        {
            AssetArchiveData archiveData = new AssetArchiveData();
            archiveData.archiveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            archiveData.archiveNotes = archiveNotes;
            archiveData.archiveTag = archiveTag;
            archiveData.moduleCompileInfoList = moduleCompileInfoList;
            return archiveData;
        }
    }
}
