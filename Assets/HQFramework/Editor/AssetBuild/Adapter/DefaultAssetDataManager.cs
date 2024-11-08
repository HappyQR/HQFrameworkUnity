using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetDataManager
    {
        private static readonly string moduleCompileDataFileName = "BuildHistory.data";
        private static readonly string assetArchiveFileName = "Archives.data";
        private static readonly string publishDataFileName = "Publish.data";
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

        private string publishDataFilePath
        {
            get
            {
                string archiveDir = Path.Combine(Application.dataPath, HQAssetBuildLauncher.CurrentBuildConfig.assetOutputDir, "Archive");
                if (!Directory.Exists(archiveDir))
                {
                    Directory.CreateDirectory(archiveDir);
                }
                return Path.Combine(archiveDir, publishDataFileName);
            }
        }

        private List<AssetArchiveData> archiveDataList;
        private List<AssetModuleCompileInfo> compileInfoList;
        private List<HQAssetManifest> publishDataList;

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

        public async Task<bool> AddPublishDataAsync(HQAssetManifest publishData)
        {
            if (publishDataList == null)
            {
                publishDataList = await GetAssetPublishHistoryAsync();
            }
            publishDataList.Add(publishData);
            byte[] data = AssetUtility.ConfigSerializer.Serialize(publishDataList);
            await File.WriteAllBytesAsync(publishDataFilePath, data);
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

        public async Task<List<HQAssetManifest>> GetAssetPublishHistoryAsync()
        {
            if (publishDataList != null)
            {
                return publishDataList;
            }
            else if (!File.Exists(publishDataFilePath))
            {
                publishDataList = new List<HQAssetManifest>();
            }
            else
            {
                byte[] data = await File.ReadAllBytesAsync(publishDataFilePath);
                publishDataList = AssetUtility.ConfigSerializer.Deserialize<List<HQAssetManifest>>(data);
            }

            return publishDataList;
        }

        public void Dispose()
        {
            archiveDataList = null;
            compileInfoList = null;
        }
    }
}
