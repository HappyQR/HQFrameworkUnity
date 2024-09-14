namespace HQFramework.Resource
{
    public class HotfixDownloadCancelEventArgs : IReference
    {
        public int HotfixID { get; private set; }

        public int CancelCount { get; private set; }

        public static HotfixDownloadCancelEventArgs Create(int hotfixID, int cancelCount)
        {
            HotfixDownloadCancelEventArgs args = ReferencePool.Spawn<HotfixDownloadCancelEventArgs>();
            args.HotfixID = hotfixID;
            args.CancelCount = cancelCount;
            return args;
        }

        void IReference.OnRecyle()
        {
            HotfixID = 0;
            CancelCount = 0;
        }
    }
}
