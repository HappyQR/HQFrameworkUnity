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
        private OssClient client;

        private static string manifestUrl = "HQFramework/Assets/AssetModuleManifest.json";
        private static string bucketName = "happyq-test";
        private static string assetUrlRoot = "HQFramework/Assets";

        private static string urlRoot = "https://happyq-test.oss-cn-beijing.aliyuncs.com/";

        public string AssetsBuiltinDir => Path.Combine(Application.streamingAssetsPath, HQAssetBuildLauncher.CurrentBuildConfig.assetBuiltinDir);

        private void Init()
        {
            if (client == null)
            {
                string[] key_id = File.ReadAllText(Path.Combine(Application.dataPath, "../Build/Aliyun.txt")).Split('|');
                string accessId = key_id[0];
                string accessKey = key_id[1];
                string endpoint = "https://oss-cn-beijing.aliyuncs.com";
                string region = "cn-beijing";
                ClientConfiguration conf = new ClientConfiguration();
                conf.SignatureVersion = SignatureVersion.V4;
                conf.ConnectionTimeout = 3000;
                conf.EnalbeMD5Check = true;
                client = new OssClient(endpoint, accessId, accessKey, conf);
                client.SetRegion(region);
            }
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
            return Path.Combine(urlRoot, assetUrlRoot, moduleInfo.moduleName);
        }

        public Task<AssetModuleManifest> GetRemoteManifestAsync()
        {
            return GetRemoteManifestInternal();
        }

        private async Task<AssetModuleManifest> GetRemoteManifestInternal()
        {
            Init();
            if (!client.DoesObjectExist(bucketName, manifestUrl))
            {
                AssetModuleManifest remoteManifest = new AssetModuleManifest();
                remoteManifest.moduleDic = new Dictionary<int, AssetModuleInfo>();
                return remoteManifest;
            }
            else
            {
                string manifestFileUrl = Path.Combine(urlRoot, manifestUrl);
                using HttpClient httpClient = new HttpClient();
                string jsonStr = await httpClient.GetStringAsync(manifestFileUrl);
                return JsonUtilityEditor.ToObject<AssetModuleManifest>(jsonStr);
            }
        }

        public Task<bool> UploadBundleAsync(AssetBundleUploadItem item)
        {
            return Task.Run(() => UploadBundleInternal(item));
        }

        private bool UploadBundleInternal(AssetBundleUploadItem item)
        {
            Init();
            string bundleUrl = Path.Combine(assetUrlRoot, item.moduleInfo.moduleName, item.bundleInfo.bundleName);
            using PutObjectResult result = client.PutObject(bucketName, bundleUrl, item.bundleFilePath);
            return result.HttpStatusCode == HttpStatusCode.OK;
        }

        public Task<bool> UploadManifestAsync(AssetModuleManifest manifest)
        {
            return Task.Run(() => UploadManifestInternal(manifest));
        }

        private bool UploadManifestInternal(AssetModuleManifest manifest)
        {
            Init();
            string jsonStr = JsonUtilityEditor.ToJson(manifest);
            byte[] data = Encoding.UTF8.GetBytes(jsonStr);
            using Stream stream = new MemoryStream(data);
            using PutObjectResult result = client.PutObject(bucketName, manifestUrl, stream);
            return result.HttpStatusCode == HttpStatusCode.OK;
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
