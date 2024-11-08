namespace HQFramework.Resource
{
    public readonly struct AssetBundleInfo
    {
        public readonly string bundleName;
        public readonly int refCount;
        public readonly bool ready;

        public AssetBundleInfo(string bundleName, int refCount, bool ready)
        {
            this.bundleName = bundleName;
            this.refCount = refCount;
            this.ready = ready;
        }
    }
}
