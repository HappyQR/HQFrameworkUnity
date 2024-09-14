using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetRuntimeConfig : ScriptableObject
    {
        public string tag;

        public AssetHotfixMode hotfixMode;

        public string assetPersistentDir;

        public string assetBuiltinDir;

        public string hotfixManifestUrl;
    }
}
