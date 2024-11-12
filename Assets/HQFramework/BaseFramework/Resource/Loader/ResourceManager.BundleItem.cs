namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class BundleItem
        {
            public uint crc;
            public int refCount;
            public object bundleObject;
            public ResourceStatus status;

            public BundleItem(uint crc)
            {
                this.crc = crc;
                this.status = ResourceStatus.Pending;
                this.refCount = 0;
                this.bundleObject = null;
            }
        }
    }
}
