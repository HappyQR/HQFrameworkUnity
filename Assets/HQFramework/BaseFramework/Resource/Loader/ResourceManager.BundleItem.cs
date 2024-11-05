namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class BundleItem
        {
            public int refCount;
            public object bundleObject;

            public bool Ready => bundleObject != null;

            public BundleItem()
            {
                refCount = 0;
                bundleObject = null;
            }
        }
    }
}
