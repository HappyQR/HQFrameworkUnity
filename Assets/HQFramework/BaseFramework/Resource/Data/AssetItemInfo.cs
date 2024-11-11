namespace HQFramework.Resource
{
    public readonly struct AssetItemInfo
    {
        public readonly uint crc;
        public readonly string assetPath;
        public readonly int refCount;
        public readonly ResourceStatus status;

        public AssetItemInfo(uint crc, string assetPath, int refCount, ResourceStatus status)
        {
            this.crc = crc;
            this.assetPath = assetPath;
            this.refCount = refCount;
            this.status = status;
        }
    }
}
