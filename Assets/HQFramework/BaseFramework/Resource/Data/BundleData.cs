namespace HQFramework.Resource
{
    public readonly struct BundleData
    {
        public readonly string bundleName;
        public readonly int refCount;
        public readonly bool ready;

        public BundleData(string bundleName, int refCount, bool ready)
        {
            this.bundleName = bundleName;
            this.refCount = refCount;
            this.ready = ready;
        }
    }
}
