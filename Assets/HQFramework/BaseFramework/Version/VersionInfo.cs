using System;

namespace HQFramework.Version
{
    [Serializable]
    public class VersionInfo
    {
        public string productName;
        public string companyName;
        public string runtimePlatform;
        public string productVersion; // x.x.x
        public int internalVersionCode; // int value, auto-increment
        public int minimalSupportedVersionCode; // int value
        public string releaseNote;
    }
}
