using System.Threading.Tasks;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    public interface IAssetPublishHelper
    {
        AssetModuleManifest GetBasicManifest();

        string GetBundleRelatedUrl(AssetBundleInfo bundleInfo, AssetModuleInfo moduleInfo);

        string GetModuleUrlRoot(AssetModuleInfo moduleInfo);

        Task<AssetModuleManifest> GetRemoteManifestAsync();

        Task<bool> UploadBundleAsync(AssetBundleUploadItem item);

        Task<bool> UploadManifestAsync(AssetModuleManifest manifest);
    }
}
