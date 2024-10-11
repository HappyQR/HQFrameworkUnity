using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetDataManager
    {
        private static readonly string moduleCompileDataFileName = "BuildHistory.data";
        private static readonly string assetArchiveFileName = "Archives.data";
        private string archiveFilePath
        {
            get
            {
                string archiveDir = Path.Combine(Application.dataPath, HQAssetBuildLauncher.CurrentBuildConfig.assetOutputDir, "Archive");
                if (!Directory.Exists(archiveDir))
                {
                    Directory.CreateDirectory(archiveDir);
                }
                return Path.Combine(archiveDir, assetArchiveFileName);
            }
        }

        private string compileDataFilePath
        {
            get
            {
                string archiveDir = Path.Combine(Application.dataPath, HQAssetBuildLauncher.CurrentBuildConfig.assetOutputDir, "Archive");
                if (!Directory.Exists(archiveDir))
                {
                    Directory.CreateDirectory(archiveDir);
                }
                return Path.Combine(archiveDir, moduleCompileDataFileName);
            }
        }

        private List<AssetArchiveData> archiveDataList;
        private List<AssetModuleCompileInfo> compileInfoList;

        public async Task<bool> AddAssetArchiveAsync(AssetArchiveData archive)
        {
            if (archiveDataList == null)
            {
                archiveDataList = await GetAssetArchivesAsync();
            }
            archiveDataList.Add(archive);
            byte[] data = AssetUtility.ConfigSerializer.Serialize(archiveDataList);
            await File.WriteAllBytesAsync(archiveFilePath, data);
            return true;
        }

        public async Task<bool> AddAssetModuleCompileInfosAsync(List<AssetModuleCompileInfo> compileInfos)
        {
            if (compileInfoList == null)
            {
                compileInfoList = await GetAssetModuleCompileHistoryAsync();
            }
            compileInfoList.AddRange(compileInfos);
            byte[] data = AssetUtility.ConfigSerializer.Serialize(compileInfoList);
            await File.WriteAllBytesAsync(compileDataFilePath, data);
            return true;
        }

        public async Task<List<AssetArchiveData>> GetAssetArchivesAsync()
        {
            if (archiveDataList != null)
            {
                return archiveDataList;
            }
            else if (!File.Exists(archiveFilePath))
            {
                archiveDataList = new List<AssetArchiveData>();
            }
            else
            {
                byte[] data = await File.ReadAllBytesAsync(archiveFilePath);
                archiveDataList = AssetUtility.ConfigSerializer.Deserialize<List<AssetArchiveData>>(data);
            }

            return archiveDataList;
        }

        public async Task<List<AssetModuleCompileInfo>> GetAssetModuleCompileHistoryAsync()
        {
            if (compileInfoList != null)
            {
                return compileInfoList;
            }
            else if (!File.Exists(compileDataFilePath))
            {
                compileInfoList = new List<AssetModuleCompileInfo>();
            }
            else
            {
                byte[] data = await File.ReadAllBytesAsync(compileDataFilePath);
                compileInfoList = AssetUtility.ConfigSerializer.Deserialize<List<AssetModuleCompileInfo>>(data);
            }

            return compileInfoList;
        }

        public void Dispose()
        {
            archiveDataList = null;
            compileInfoList = null;
        }
    }
}
