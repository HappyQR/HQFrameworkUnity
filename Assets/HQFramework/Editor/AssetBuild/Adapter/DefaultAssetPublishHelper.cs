using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetPublishHelper : IAssetPublishHelper
    {
        private string hotfixRootFolder;
        private IAssetUploader assetUploader;

        public string AssetsBuiltinDir => Path.Combine(Application.streamingAssetsPath, HQAssetBuildLauncher.CurrentBuildConfig.assetBuiltinDir);
        
        public void SetUploader(IAssetUploader uploader)
        {
            this.assetUploader = uploader;
        }
        
        public HQAssetManifest GetBasicManifest()
        {
            HQAssetManifest manifest = new HQAssetManifest();
            manifest.productName = Application.productName;
            manifest.productVersion = Application.version;

            return manifest;
        }

        public string GetBundleRelatedUrl(HQAssetBundleConfig bundleInfo, HQAssetModuleConfig moduleInfo)
        {
            return bundleInfo.bundleName;
        }

        public string GetModuleUrlRoot(HQAssetModuleConfig moduleInfo)
        {
            return "";
        }

        public Task<HQAssetManifest> GetRemoteManifestAsync()
        {
            return GetRemoteManifestInternal();
        }

        private async Task<HQAssetManifest> GetRemoteManifestInternal()
        {
            string manifestFileUrl = "";
            using HttpClient httpClient = new HttpClient();
            string jsonStr = await httpClient.GetStringAsync(manifestFileUrl);
            return JsonUtilityEditor.ToObject<HQAssetManifest>(jsonStr);
        }

        public Task<bool> UploadBundleAsync(AssetBundleUploadItem item)
        {
            return null;
        }

        public Task<bool> UploadManifestAsync(HQAssetManifest manifest)
        {
            return null;
        }

        public void PackBuiltinModule(AssetModuleCompileInfo moduleCompileInfo)
        {
            string moduleDir = Path.Combine(AssetsBuiltinDir, moduleCompileInfo.moduleID.ToString());
            Directory.CreateDirectory(moduleDir);

            for (int i = 0; i < moduleCompileInfo.bundleList.Count; i++)
            {
                AssetBundleCompileInfo bundleCompileInfo = moduleCompileInfo.bundleList[i];
                string bundleDestPath = Path.Combine(moduleDir, bundleCompileInfo.bundleName);
                File.Copy(bundleCompileInfo.filePath, bundleDestPath, true);
            }
        }
    }
}
