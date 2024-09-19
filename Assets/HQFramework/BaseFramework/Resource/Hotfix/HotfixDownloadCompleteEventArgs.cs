namespace HQFramework.Resource
{
    public class HotfixDownloadCompleteEventArgs : IReference
    {
        public int HotfixID { get; private set; }

        public static HotfixDownloadCompleteEventArgs Create(int hotfixID)
        {
            HotfixDownloadCompleteEventArgs args = ReferencePool.Spawn<HotfixDownloadCompleteEventArgs>();
            args.HotfixID = hotfixID;
            return args;
        }

        void IReference.OnRecyle()
        {
            HotfixID = 0;
        }
    }
}
