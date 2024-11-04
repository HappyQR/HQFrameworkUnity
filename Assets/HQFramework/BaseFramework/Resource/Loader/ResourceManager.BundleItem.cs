namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class BundleItem
        {
            public int refCount;
            public object bundleObject;

            public BundleItem()
            {
                refCount = 1;
                bundleObject = null;
            }
        }
    }
}
