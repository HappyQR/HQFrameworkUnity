using System.IO;
using System.Threading.Tasks;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class DefaultResourceHelper : IResourceHelper
    {
        private static readonly string resourceConfigFilePath = "ResourceConfig";
        private static readonly string manifestFileName = "AssetModuleManifest.json";

        private string localManifestFilePath;

        public int HotfixDownloadGroupID => 1;

        public ResourceConfig LoadResourceConfig()
        {
            TextAsset asset = Resources.Load<TextAsset>(resourceConfigFilePath);
            string jsonStr = asset.text;
            ResourceConfig config = SerializeManager.JsonToObject<ResourceConfig>(jsonStr);
            config.assetBuiltinDir = Path.Combine(Application.streamingAssetsPath, config.assetBuiltinDir);
            config.assetPersistentDir = Path.Combine(Application.persistentDataPath, config.assetPersistentDir);
            localManifestFilePath = Path.Combine(config.assetPersistentDir, manifestFileName);
            if (!Directory.Exists(config.assetPersistentDir))
            {
                Directory.CreateDirectory(config.assetPersistentDir);
            }
            Resources.UnloadAsset(asset);
            return config;
        }

        public async Task<AssetModuleManifest> LoadAssetManifestAsync()
        {
            string localManifestJsonStr = await File.ReadAllTextAsync(localManifestFilePath);
            AssetModuleManifest localManifest = SerializeManager.JsonToObject<AssetModuleManifest>(localManifestJsonStr);
            return localManifest;
        }

        public async Task OverrideLocalManifestAsync(AssetModuleManifest localManifest)
        {
            string manifestJson = SerializeManager.ObjectToJson(localManifest);
            await File.WriteAllTextAsync(localManifestFilePath, manifestJson);
        }
    }
}
