using System.Threading.Tasks;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetPublishHelper : IAssetPublishHelper
    {
        public AssetModuleManifest GetBasicManifest()
        {
            AssetModuleManifest manifest = new AssetModuleManifest();
            manifest.productName = Application.productName;
            manifest.productVersion = Application.version;

            return manifest;
        }

        public string GetBundleRelatedUrl(AssetBundleInfo bundleInfo, AssetModuleInfo moduleInfo)
        {
            return bundleInfo.bundleName;
        }

        public string GetModuleUrlRoot(AssetModuleInfo moduleInfo)
        {
            return moduleInfo.moduleName;
        }

        public Task<AssetModuleManifest> GetRemoteManifestAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UploadBundleAsync(AssetBundleUploadItem item)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UploadManifestAsync(AssetModuleManifest manifest)
        {
            throw new System.NotImplementedException();
        }
    }
}
