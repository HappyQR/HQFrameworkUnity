using System.IO;
using HQFramework;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class DefaultResourceHelper : IResourceHelper
    {
        private static readonly string resourceConfigFilePath = "ResourceConfig";

        public int HotfixDownloadGroupID => 1;

        public ResourceConfig LoadResourceConfig()
        {
            TextAsset asset = Resources.Load<TextAsset>(resourceConfigFilePath);
            string jsonStr = asset.text;
            ResourceConfig config = SerializeManager.JsonToObject<ResourceConfig>(jsonStr);
            config.assetBuiltinDir = Path.Combine(Application.streamingAssetsPath, config.assetBuiltinDir);
            config.assetPersistentDir = Path.Combine(Application.persistentDataPath, config.assetPersistentDir);
            if (!Directory.Exists(config.assetPersistentDir))
            {
                Directory.CreateDirectory(config.assetPersistentDir);
            }
            Resources.UnloadAsset(asset);
            return config;
        }
    }
}
