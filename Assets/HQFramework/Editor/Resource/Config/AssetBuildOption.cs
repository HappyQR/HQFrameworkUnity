using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Editor
{    
    public class AssetBuildOption : ScriptableObject
    {
        public string optionTag;
        public AssetHotfixMode hotfixMode;
        public string bundleOutputDir;
        public string builtinDir;
        public int resourceVersion;
        public int minimalSupportedVersion;
        public bool autoIncreaseResourceVersion;
        public BuildTargetPlatform platform;
        public CompressOption compressOption;

        // public bool enableEncryption;
    }

    public enum BuildTargetPlatform
    {
        Android = UnityEditor.BuildTarget.Android,
        iOS = UnityEditor.BuildTarget.iOS,
        StandaloneOSX = UnityEditor.BuildTarget.StandaloneOSX,
        StandaloneWindows = UnityEditor.BuildTarget.StandaloneWindows,
        StandaloneWindows64 = UnityEditor.BuildTarget.StandaloneWindows64,
        VisionOS = UnityEditor.BuildTarget.VisionOS,
        WebGL = UnityEditor.BuildTarget.WebGL
    }

    public enum CompressOption
    {
        LZ4 = UnityEditor.BuildAssetBundleOptions.ChunkBasedCompression,
        NoCompress = UnityEditor.BuildAssetBundleOptions.UncompressedAssetBundle
    }
}
