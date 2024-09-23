using System.IO;
using System.Threading.Tasks;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Runtime
{
    internal class DefaultResourceHelper : IResourceHelper
    {
        private static readonly string resourceConfigFilePath = "ResourceConfig";
        private static readonly string manifestFileName = "AssetModuleManifest.json";

        private ResourceConfig resourceConfig;
        private string localManifestFilePath;

        public int LauncherHotfixID => 1;

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
            this.resourceConfig = config;
            Resources.UnloadAsset(asset);
            return config;
        }

        public async Task<AssetModuleManifest> LoadAssetManifestAsync()
        {
            string localManifestJsonStr = await File.ReadAllTextAsync(localManifestFilePath);
            AssetModuleManifest localManifest = SerializeManager.JsonToObject<AssetModuleManifest>(localManifestJsonStr);
            return localManifest;
        }

        public void OverrideLocalManifest(AssetModuleManifest localManifest)
        {
            string manifestJson = SerializeManager.ObjectToJson(localManifest);
            File.WriteAllText(localManifestFilePath, manifestJson);
        }

        public string GetBundleFilePath(AssetBundleInfo bundleInfo)
        {
            return Path.Combine(resourceConfig.assetPersistentDir, bundleInfo.moduleID.ToString(), bundleInfo.md5);
        }

        public void DeleteAssetModule(AssetModuleInfo module)
        {
            string moduleDir = Path.Combine(resourceConfig.assetPersistentDir, module.id.ToString());
            if (Directory.Exists(moduleDir))
            {
                Directory.Delete(moduleDir, true);
            }
        }
    }
}
