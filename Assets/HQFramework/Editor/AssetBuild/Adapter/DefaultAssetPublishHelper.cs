using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetPublishHelper : IAssetPublishHelper
    {
        private IAssetUploader assetUploader;

        public string AssetsBuiltinDir => Path.Combine(Application.streamingAssetsPath, HQAssetBuildLauncher.CurrentBuildConfig.assetBuiltinDir);

        public IAssetUploader AssetUploader => assetUploader;

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
            return Path.Combine(assetUploader.UrlRoot, assetUploader.HotfixRootFolder, moduleInfo.moduleName);
        }

        public async Task<HQAssetManifest> GetRemoteManifestAsync()
        {
            string manifestFileUrl = Path.Combine(assetUploader.UrlRoot, assetUploader.HotfixRootFolder, assetUploader.HotfixManifestFileName);
            using HttpClient httpClient = new HttpClient();
            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync(manifestFileUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonStr = await response.Content.ReadAsStringAsync();
                    return JsonUtilityEditor.ToObject<HQAssetManifest>(jsonStr);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound || 
                        response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        HQAssetManifest newManifest = new HQAssetManifest();
                        newManifest.moduleDic = new System.Collections.Generic.Dictionary<int, HQAssetModuleConfig>();
                        return newManifest;
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<bool> UploadBundleAsync(AssetBundleUploadItem item)
        {
            string relatedUrl = Path.Combine(item.moduleInfo.moduleName, item.bundleInfo.bundleName);
            return assetUploader.UploadAssetAsync(relatedUrl, item.bundleFilePath);
        }

        public Task<bool> UploadManifestAsync(HQAssetManifest manifest)
        {
            string jsonStr = JsonUtilityEditor.ToJson(manifest);
            byte[] content = Encoding.UTF8.GetBytes(jsonStr);
            string relatedUrl = assetUploader.HotfixManifestFileName;
            return assetUploader.UploadAssetAsync(relatedUrl, content);
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
