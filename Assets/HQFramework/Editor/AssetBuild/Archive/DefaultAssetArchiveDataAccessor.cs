using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetArchiveDataAccessor : IAssetArchiveDataAccessor
    {
        private static readonly string archiveFolderName = "Archives";
        private static readonly string buildHistoryFileName = "BuildHistory.data";
        private static readonly string archiveDataFileName = "Archive.data";

        private string ArchiveDir
        {
            get
            {
                string dir = Path.Combine(Application.dataPath, AssetConfigManager.CurrentBuildConfig.assetOutputDir, archiveFolderName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }

        public async Task AddModuleBuildResults(AssetModuleBuildResult[] moduleBuildResultArr)
        {
            AssetModuleBuildHistoryData historyData = await LoadModuleBuildHistoryData();
            for (int i = 0; i < moduleBuildResultArr.Length; i++)
            {
                AssetModuleBuildResult moduleBuildResult = moduleBuildResultArr[i];
                if (!historyData.moduleBuildData.ContainsKey(moduleBuildResult.moduleID))
                {
                    historyData.moduleBuildData.Add(moduleBuildResult.moduleID, new List<AssetModuleBuildResult>());
                }
                historyData.moduleBuildData[moduleBuildResult.moduleID].Add(moduleBuildResult);
            }

            byte[] bytes = AssetUtility.ConfigSerializer.Serialize(historyData);
            await File.WriteAllBytesAsync(Path.Combine(ArchiveDir, buildHistoryFileName), bytes);
        }

        public async Task<AssetArchiveData> LoadAssetArchiveData()
        {
            string filePath = Path.Combine(ArchiveDir, archiveDataFileName);
            if (!File.Exists(filePath))
            {
                return new AssetArchiveData();
            }
            byte[] bytes = await File.ReadAllBytesAsync(filePath);
            return AssetUtility.ConfigSerializer.Deserialize<AssetArchiveData>(bytes);
        }

        public async Task<AssetModuleBuildHistoryData> LoadModuleBuildHistoryData()
        {
            string filePath = Path.Combine(ArchiveDir, buildHistoryFileName);
            if (!File.Exists(filePath))
            {
                return new AssetModuleBuildHistoryData();
            }
            byte[] bytes = await File.ReadAllBytesAsync(filePath);
            return AssetUtility.ConfigSerializer.Deserialize<AssetModuleBuildHistoryData>(bytes);
        }

        public async void SaveAssetArchiveData(AssetArchiveData archiveData)
        {
            byte[] bytes = AssetUtility.ConfigSerializer.Serialize(archiveData);
            string filePath = Path.Combine(ArchiveDir, archiveDataFileName);
            await File.WriteAllBytesAsync(filePath, bytes);
        }
    }
}
