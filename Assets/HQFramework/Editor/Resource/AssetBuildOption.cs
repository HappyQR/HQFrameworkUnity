using UnityEngine;

namespace HQFramework.Editor
{
    [CreateAssetMenu(fileName = "AssetBuildOption", menuName = "HQFramework/AssetBuildOption", order = 0)]
    public class AssetBuildOption : ScriptableObject
    {
        public string tag;
        public bool enableHotfix;
        public string bundleOutputDir;
        public string manifestOutputDir;
        public string builtinDir;
        public string bundleUploadUrl;
        public string manifestUploadUrl;
        public bool enableEncryption;
        public CompressOption compressOption;
        public BuildTargetPlatform platform;

        // 资源大版本号，通常unity升级、工程框架重大变化、资源管理/加密方式变化等时才更新
        public int genericVersion = 1;
    }

    public enum BuildTargetPlatform
    {
        Android = 13,
        iOS = 9,
        StandaloneOSX = 2,
        StandaloneWindows = 5
    }

    public enum CompressOption
    {
        LZ4 = 0x100,
        LZMA = 0,
        NoCompress = 1
    }
}