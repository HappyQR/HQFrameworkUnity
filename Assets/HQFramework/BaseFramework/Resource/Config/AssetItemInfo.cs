using System;

namespace HQFramework.Resource
{
    [Serializable]
    public class AssetItemInfo
    {
        public uint crc;
        public string assetPath;
        public string assetName;
        public int moduleID;
        public string bundleName;
    }

}