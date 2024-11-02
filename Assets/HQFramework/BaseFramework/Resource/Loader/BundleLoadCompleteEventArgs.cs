namespace HQFramework.Resource
{
    public class BundleLoadCompleteEventArgs : IReference
    {
        public string bundleName
        {
            get;
            private set;
        }

        public object bundleObject
        {
            get;
            private set;
        }

        public static BundleLoadCompleteEventArgs Create(string bundleName, object bundleObject)
        {
            BundleLoadCompleteEventArgs args = ReferencePool.Spawn<BundleLoadCompleteEventArgs>();
            args.bundleName = bundleName;
            args.bundleObject = bundleObject;
            return args;
        }

        void IReference.OnRecyle()
        {
            bundleName = null;
            bundleObject = null;
        }
    }
}
