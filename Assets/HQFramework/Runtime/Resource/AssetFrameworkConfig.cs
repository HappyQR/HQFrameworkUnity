using UnityEngine;

namespace HQFramework.Runtime.Resource
{
    [CreateAssetMenu(fileName = "AssetFrameworkConfig", menuName = "HQFramework/AssetFrameworkConfig", order = 0)]
    public class AssetFrameworkConfig : ScriptableObject
    {
        public bool enableHotfix;
        public string hotfixUrl;
        public string hotfixManifestUrl;

        public string builtinDir;
        public string assetPersistentDir;
        public int maxDownloadThreadCount;
    }
}