using System.Threading.Tasks;

namespace HQFramework.Editor
{
    public interface IAssetArchiveDataAccessor
    {
        Task AddModuleBuildResults(AssetModuleBuildResult[] moduleBuildResultArr);

        Task<AssetModuleBuildHistoryData> LoadModuleBuildHistoryData();

        Task<AssetArchiveData> LoadAssetArchiveData();

        void SaveAssetArchiveData(AssetArchiveData archiveData);
    }
}
