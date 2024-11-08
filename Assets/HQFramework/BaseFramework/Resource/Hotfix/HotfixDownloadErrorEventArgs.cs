namespace HQFramework.Resource
{
    public class HotfixDownloadErrorEventArgs : IReference
    {
        public int HotfixID { get; private set; }
        public HQAssetBundleConfig BundleInfo { get; private set; }
        public string ErrorMessage { get; private set; }

        public static HotfixDownloadErrorEventArgs Create(int hotfixID, HQAssetBundleConfig bundleInfo, string errorMessage)
        {
            HotfixDownloadErrorEventArgs args = ReferencePool.Spawn<HotfixDownloadErrorEventArgs>();
            args.HotfixID = hotfixID;
            args.BundleInfo = bundleInfo;
            args.ErrorMessage = errorMessage;
            return args;
        }

        void IReference.OnRecyle()
        {
            HotfixID = -1;
            BundleInfo = null;
            ErrorMessage = null;
        }
    }
}
