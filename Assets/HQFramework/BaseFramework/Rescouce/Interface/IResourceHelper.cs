using System;
using System.Threading.Tasks;

namespace HQFramework.Resource
{
    public interface IResourceHelper
    {
        int HotfixDownloadGroupID
        {
            get;
        }
        
        ResourceConfig LoadResourceConfig();

        Task<AssetModuleManifest> LoadAssetManifestAsync();

        Task OverrideLocalManifestAsync(AssetModuleManifest localManifest);
    }
}
