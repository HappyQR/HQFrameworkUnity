namespace HQFramework.Resource
{
    public class BundleLoadCompleteEventArgs : IReference
    {
        public uint bundleID
        {
            get;
            private set;
        }

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

        public static BundleLoadCompleteEventArgs Create(uint bundleID, string bundleName, object bundleObject)
        {
            BundleLoadCompleteEventArgs args = ReferencePool.Spawn<BundleLoadCompleteEventArgs>();
            args.bundleName = bundleName;
            args.bundleObject = bundleObject;
            args.bundleID = bundleID;
            return args;
        }

        void IReference.OnRecyle()
        {
            bundleID = 0;
            bundleName = null;
            bundleObject = null;
        }
    }
}
