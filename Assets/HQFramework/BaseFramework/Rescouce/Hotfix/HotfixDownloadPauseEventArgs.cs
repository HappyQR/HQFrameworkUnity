namespace HQFramework.Resource
{
    public class HotfixDownloadPauseEventArgs : IReference
    {
        public int HotfixID { get; private set; }

        public int PauseCount { get; private set; }

        public static HotfixDownloadPauseEventArgs Create(int hotfixID, int pauseCount)
        {
            HotfixDownloadPauseEventArgs args = ReferencePool.Spawn<HotfixDownloadPauseEventArgs>();
            args.HotfixID = hotfixID;
            args.PauseCount = pauseCount;
            return args;
        }

        void IReference.OnRecyle()
        {
            HotfixID = 0;
            PauseCount = 0;
        }
    }
}
