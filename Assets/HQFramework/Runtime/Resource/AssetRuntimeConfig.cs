using UnityEngine;

namespace HQFramework.Runtime.Resource
{
    [CreateAssetMenu(fileName = "AssetRuntimeConfig", menuName = "HQFramework/AssetRuntimeConfig", order = 0)]
    public class AssetRuntimeConfig : ScriptableObject
    {
        public bool enableHotfix;
        public string hotfixUrl;
        public string hotfixManifestUrl;

        public string builtinDir;
        public string assetPersistentDir;
        public int maxDownloadThreadCount;
    }
}