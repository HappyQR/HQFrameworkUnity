using System;

namespace HQFramework.Resource
{
    [Serializable]
    public class AssetItemInfo
    {
        public string assetPath;
        public string assetName;
        public uint crc;
        public int moduleID;
        public string bundleName;
    }

}