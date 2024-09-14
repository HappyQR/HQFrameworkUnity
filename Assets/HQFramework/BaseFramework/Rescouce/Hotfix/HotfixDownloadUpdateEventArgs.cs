namespace HQFramework.Resource
{
    public class HotfixDownloadUpdateEventArgs : IReference
    {
        public int HotfixID { get; private set; }
        public int DownloadedSize { get; private set; }
        public int TotalSize { get; private set;}

        public static HotfixDownloadUpdateEventArgs Create(int htofixID, int downloadedSize, int totalSize)
        {
            HotfixDownloadUpdateEventArgs args = ReferencePool.Spawn<HotfixDownloadUpdateEventArgs>();
            args.HotfixID = htofixID;
            args.DownloadedSize = downloadedSize;
            args.TotalSize = totalSize;
            return args;
        }

        void IReference.OnRecyle()
        {
            HotfixID = 0;
            DownloadedSize = 0;
            TotalSize = 0;
        }
    }
}
