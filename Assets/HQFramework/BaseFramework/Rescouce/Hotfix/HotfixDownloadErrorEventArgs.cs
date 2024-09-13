namespace HQFramework.Resource
{
    public class HotfixDownloadErrorEventArgs : IReference
    {
        public int ModuleID { get; private set; }
        public string BundleName { get; private set; }
        public string ErrorMessage { get; private set; }

        public static HotfixDownloadErrorEventArgs Create(int moduleID, string bundleName, string errorMessage)
        {
            HotfixDownloadErrorEventArgs args = ReferencePool.Spawn<HotfixDownloadErrorEventArgs>();
            args.ModuleID = moduleID;
            args.BundleName = bundleName;
            args.ErrorMessage = errorMessage;
            return args;
        }

        void IReference.OnRecyle()
        {
            ModuleID = -1;
            BundleName = null;
            ErrorMessage = null;
        }
    }
}
