using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Editor
{
    [CreateAssetMenu(fileName = "AssetBuildOption", menuName = "HQFramework/AssetBuildOption", order = 0)]
    public class AssetBuildOption : ScriptableObject
    {
        public string optionId;
        public string tag;
        public int resourceVersion;
        public bool autoIncreaseResourceVersion;
        public AssetHotfixMode hotfixMode;
        public string bundleOutputDir;
        public string manifestOutputDir;
        public string builtinDir;
        // public string bundleUploadUrl;
        // public string manifestUploadUrl;
        public CompressOption compressOption;
        public BuildTargetPlatform platform;

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