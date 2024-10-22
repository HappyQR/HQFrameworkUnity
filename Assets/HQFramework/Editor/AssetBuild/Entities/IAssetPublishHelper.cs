using System.Threading.Tasks;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    public interface IAssetPublishHelper
    {
        string GetBundleRelatedUrl(AssetBundleInfo bundleInfo, AssetModuleInfo moduleInfo);

        string GetModuleUrlRoot(AssetModuleInfo moduleInfo);

        Task<bool> UploadFile(string filePath, string relatedUrl);
    }
}
