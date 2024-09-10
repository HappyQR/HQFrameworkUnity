using System.IO;
using HQFramework;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class DefaultResourceHelper : IResourceHelper
    {
        private static readonly string resourceConfigFilePath = "ResourceConfig";

        public ResourceConfig LoadResourceConfig()
        {
            string jsonStr = Resources.Load<TextAsset>(resourceConfigFilePath).text;
            ResourceConfig config = SerializeManager.JsonToObject<ResourceConfig>(jsonStr);
            config.assetBuiltinDir = Path.Combine(Application.streamingAssetsPath, config.assetBuiltinDir);
            config.assetPersistentDir = Path.Combine(Application.persistentDataPath, config.assetPersistentDir);
            return config;
        }
    }
}
