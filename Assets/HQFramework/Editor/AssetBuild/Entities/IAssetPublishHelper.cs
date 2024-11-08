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

        HQAssetManifest GetBasicManifest();

        string GetBundleRelatedUrl(HQAssetBundleConfig bundleInfo, HQAssetModuleConfig moduleInfo);

        string GetModuleUrlRoot(HQAssetModuleConfig moduleInfo);

        void PackBuiltinModule(AssetModuleCompileInfo moduleCompileInfo);

        Task<HQAssetManifest> GetRemoteManifestAsync();

        Task<bool> UploadBundleAsync(AssetBundleUploadItem item);

        Task<bool> UploadManifestAsync(HQAssetManifest manifest);
    }
}
