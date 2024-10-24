using System.IO;
using System.Threading.Tasks;
using HQFramework.Resource;

namespace HQFramework.Runtime
{
    internal class DefaultResourceHelper : IResourceHelper
    {
        private static readonly string resourceConfigFilePath = "ResourceConfig";
        private static readonly string manifestFileName = "AssetModuleManifest.json";

        private string localManifestFilePath;

        public int LauncherHotfixID => 1;

        public AssetHotfixMode HotfixMode => throw new System.NotImplementedException();

        public string AssetsPersistentDir => throw new System.NotImplementedException();

        public string AssetsBuiltinDir => throw new System.NotImplementedException();

        public string HotfixManifestUrl => throw new System.NotImplementedException();

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
            return Path.Combine(AssetsPersistentDir, bundleInfo.moduleID.ToString(), bundleInfo.bundleName);
        }

        public void DeleteAssetModule(AssetModuleInfo module)
        {
            string moduleDir = Path.Combine(AssetsPersistentDir, module.id.ToString());
            if (Directory.Exists(moduleDir))
            {
                Directory.Delete(moduleDir, true);
            }
        }
    }
}
