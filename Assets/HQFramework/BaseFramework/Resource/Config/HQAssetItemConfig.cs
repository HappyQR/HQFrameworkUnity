using System;

namespace HQFramework.Resource
{
    [Serializable]
    public class HQAssetItemConfig
    {
        public uint crc;
        public int moduleID;
        public uint bundleID;
        public string bundleName;
        public string assetPath;
        public string assetName;
        public uint[] dependencies;
    }
}