using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AmazonAssetPublishHelper : IAssetPublishHelper
    {
        private AmazonS3Client client;

        private static string urlRoot = "https://assets.moonvrhome.com";
        private static string manifestFileUrl = "dev/hotfix-framework/AssetModuleManifest.json";
        private static string bucketName = "assets.moonvrhome.com";

        public string AssetsBuiltinDir => Path.Combine(Application.streamingAssetsPath, HQAssetBuildLauncher.CurrentBuildConfig.assetBuiltinDir);

        private void Init()
        {
            if (client != null)
            {
                return;
            }

            string[] key_id = File.ReadAllText(Path.Combine(Application.dataPath, "../Build/Amazon.txt")).Split('|');
            string accessId = key_id[0];
            string accessKey = key_id[1];

            AmazonS3Config config = new AmazonS3Config();
            config.RegionEndpoint = RegionEndpoint.EUWest2;
            config.Timeout = TimeSpan.FromSeconds(10);
            client = new AmazonS3Client(accessId, accessKey, config);
        }

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
            return Path.Combine(urlRoot, moduleInfo.moduleName);
        }

        public Task<AssetModuleManifest> GetRemoteManifestAsync()
        {
            return GetRemoteManifestInternal();
        }

        private async Task<AssetModuleManifest> GetRemoteManifestInternal()
        {
            Init();
            using GetObjectResponse request = await client.GetObjectAsync(bucketName, manifestFileUrl);
            Debug.Log(request.HttpStatusCode);
            client.Dispose();
            return null;
        }

        public void PackBuiltinModule(AssetModuleCompileInfo moduleCompileInfo)
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
