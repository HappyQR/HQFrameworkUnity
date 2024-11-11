namespace HQFramework.Resource
{
    public readonly struct AssetBundleInfo
    {
        public readonly uint bundleID;
        public readonly string bundleName;
        public readonly int refCount;
        public readonly ResourceStatus status;

        public AssetBundleInfo(uint bundleID, string bundleName, int refCount, ResourceStatus status)
        {
            this.bundleID = bundleID;
            this.bundleName = bundleName;
            this.refCount = refCount;
            this.status = status;
        }
    }
}
