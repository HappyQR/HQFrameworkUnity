namespace HQFramework.Resource
{
    public class HotfixDownloadResumeEventArgs : IReference
    {
        public int HotfixID { get; private set; }

        public int ResumeCount { get; private set; }

        public static HotfixDownloadResumeEventArgs Create(int hotfixID, int resumeCount)
        {
            HotfixDownloadResumeEventArgs args = ReferencePool.Spawn<HotfixDownloadResumeEventArgs>();
            args.HotfixID = hotfixID;
            args.ResumeCount = resumeCount;
            return args;
        }

        void IReference.OnRecyle()
        {
            HotfixID = 0;
            ResumeCount = 0;
        }
    }
}
