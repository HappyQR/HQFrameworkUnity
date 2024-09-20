using UnityEngine;

namespace HQFramework.Editor
{
    public class AppBuildConfig : ScriptableObject
    {
        public string tag;
        public string productName;
        public string companyName;
        public string runtimePlatform;
        public string productVersion;
        public string versionTag;
        public bool devBuild;
        public string bundleOutputDir;
        public string buildBundleName;
        public int internalVersionCode;
        public int nextVersionCode;
        public bool autoIncreaseBuildVersion;
        public int minimalSupportedVersionCode;
        public string releaseNote;
    }
}
