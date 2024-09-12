using System;

namespace HQFramework.Resource
{
    [Serializable]
    public class ResourceConfig
    {
        public AssetHotfixMode hotfixMode;

        public string assetPersistentDir;

        public string assetBuiltinDir;

        public string hotfixUrl;

        public string hotfixManifestUrl;
    }
}
