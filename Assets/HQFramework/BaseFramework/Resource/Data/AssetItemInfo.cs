namespace HQFramework.Resource
{
    public readonly struct AssetItemInfo
    {
        public readonly uint crc;
        public readonly int moduleID;
        public readonly string bundleName;
        public readonly string path;

        public AssetItemInfo(uint crc, int moduleID, string bundleName, string path)
        {
            this.crc = crc;
            this.moduleID = moduleID;
            this.bundleName = bundleName;
            this.path = path;
        }
    }
}
