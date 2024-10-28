using System;

namespace HQFramework.Editor
{
    [Serializable]
    public class AssetBuildConfig
    {
        public string tag;

        // Asset Build
        public string assetOutputDir;
        public string preprocessorName;
        public string compilerName;
        public string postprocessorName;
        public BuildTargetPlatform platform;
        public CompressOption compressOption;
        public bool enableEncryption; // TODO: will be implemented in the future.

        // Asset Archive
        public string assetBuiltinDir;

        // Asset Publish
        public string publishHelperName;
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
