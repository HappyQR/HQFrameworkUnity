using System;


namespace HQFramework.Resource
{
    [Serializable]
    public class HQAssetBundleConfig
    {
        public uint crc;
        public int moduleID;
        public string moduleName;
        public string bundleName;
        public string md5;
        public int size; // unit : byte
        public string bundleUrlRelatedToModule;
        public uint[] dependencies;
    }
}