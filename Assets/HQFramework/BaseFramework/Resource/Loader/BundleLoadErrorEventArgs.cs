namespace HQFramework.Resource
{
    public class BundleLoadErrorEventArgs : IReference
    {
        public string bundleName
        {
            get;
            private set;
        }

        public string errorMessage
        {
            get;
            private set;
        }

        public static BundleLoadErrorEventArgs Create(string bundleName, string errorMessage)
        {
            BundleLoadErrorEventArgs args = ReferencePool.Spawn<BundleLoadErrorEventArgs>();
            args.bundleName = bundleName;
            args.errorMessage = errorMessage;
            return args;
        }

        void IReference.OnRecyle()
        {
            bundleName = null;
            errorMessage = null;
        }
    }
}
