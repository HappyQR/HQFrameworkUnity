using System.Threading.Tasks;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    public interface IAssetPublishHelper
    {
        string AssetsBuiltinDir
        {
            get;
        }

        void SetUploader(IAssetUploader uploader);

        AssetModuleManifest GetBasicManifest();

        string GetBundleRelatedUrl(AssetBundleInfo bundleInfo, AssetModuleInfo moduleInfo);

        string GetModuleUrlRoot(AssetModuleInfo moduleInfo);

        void PackBuiltinModule(AssetModuleCompileInfo moduleCompileInfo);

        Task<AssetModuleManifest> GetRemoteManifestAsync();

        Task<bool> UploadBundleAsync(AssetBundleUploadItem item);

        Task<bool> UploadManifestAsync(AssetModuleManifest manifest);
    }
}
