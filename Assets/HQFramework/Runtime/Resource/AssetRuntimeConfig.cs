using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Runtime.Resource
{
    [CreateAssetMenu(fileName = "AssetRuntimeConfig", menuName = "HQFramework/AssetRuntimeConfig", order = 0)]
    public class AssetRuntimeConfig : ScriptableObject
    {
        public AssetHotfixMode hotfixMode;
        public string hotfixUrl;
        public string hotfixManifestUrl;

        public string builtinDir;
        public string assetPersistentDir;
    }
}