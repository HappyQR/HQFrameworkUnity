using System;


namespace HQFramework.Resource
{
    [Serializable]
    public class HQAssetBundleConfig
    {
        public int moduleID;
        public string moduleName;
        public string bundleName;
        public string md5;
        public int size; // unit : byte
        public string[] dependencies;
        public string bundleUrlRelatedToModule;
    }
}