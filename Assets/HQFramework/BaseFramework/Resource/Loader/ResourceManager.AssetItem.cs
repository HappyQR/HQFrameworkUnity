namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class AssetItem
        {
            public uint crc;
            public int refCount;
            public object assetObject;
            public ResourceStatus status;

            public AssetItem(uint crc)
            {
                this.crc = crc;
                this.status = ResourceStatus.Pending;
                this.refCount = 0;
                this.assetObject = null;
            }
        }
    }
}
