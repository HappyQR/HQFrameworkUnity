using System.IO;
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

        public void PackBuiltinModule(AssetModuleCompileInfo moduleCompileInfo)
        {
            string moduleDir = Path.Combine(Application.streamingAssetsPath, HQAssetBuildLauncher.CurrentBuildConfig.assetBuiltinDir, moduleCompileInfo.moduleID.ToString());
            if (Directory.Exists(moduleDir))
            {
                Directory.Delete(moduleDir, true);
            }
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
