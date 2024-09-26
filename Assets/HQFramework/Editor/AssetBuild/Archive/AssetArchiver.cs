using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HQFramework.Editor
{
    public sealed class AssetArchiver
    {
        public static async void AddModuleBuildeResults(AssetModuleBuildResult[] moduleBuildResultArr)
        {
            IAssetArchiveDataAccessor accessor = new DefaultAssetArchiveDataAccessor();

            await accessor.AddModuleBuildResults(moduleBuildResultArr);
        }

        public static async Task<AssetModuleBuildHistoryData> LoadBuildHistoryData()
        {
            IAssetArchiveDataAccessor accessor = new DefaultAssetArchiveDataAccessor();

            return await accessor.LoadModuleBuildHistoryData();
        }

        public static async void PackArchive(List<AssetModuleBuildResult> moduleBuildResults, string archiveNotes = null)
        {
            IAssetArchiveDataAccessor accessor = new DefaultAssetArchiveDataAccessor();

            AssetArchiveData data = await accessor.LoadAssetArchiveData();
            data.assetArchiveList.Sort((item1, item2) => item1.archiveVersionCode < item2.archiveVersionCode ? -1 : 1);
            AssetArchiveInfo archiveInfo = new AssetArchiveInfo();
            archiveInfo.archiveNotes = archiveNotes;
            archiveInfo.archiveTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            if (data.assetArchiveList.Count > 0)
            {
                archiveInfo.archiveVersionCode = data.assetArchiveList[data.assetArchiveList.Count - 1].archiveVersionCode + 1;
            }
            else
            {
                archiveInfo.archiveVersionCode = 0;
            }
            archiveInfo.moduleBuildResultDic = new Dictionary<int, AssetModuleBuildResult>();
            for (int i = 0; i < moduleBuildResults.Count; i++)
            {
                archiveInfo.moduleBuildResultDic.Add(moduleBuildResults[i].moduleID, moduleBuildResults[i]);
            }
            data.assetArchiveList.Add(archiveInfo);
            accessor.SaveAssetArchiveData(data);
        }
    }
}
