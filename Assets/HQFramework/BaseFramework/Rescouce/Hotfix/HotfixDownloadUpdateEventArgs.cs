namespace HQFramework.Resource
{
    public class HotfixDownloadUpdateEventArgs : IReference
    {
        public int DownloadedSize { get; private set; }
        public int TotalSize { get; private set;}

        public static HotfixDownloadUpdateEventArgs Create(int downloadedSize, int totalSize)
        {
            HotfixDownloadUpdateEventArgs args = ReferencePool.Spawn<HotfixDownloadUpdateEventArgs>();
            args.DownloadedSize = downloadedSize;
            args.TotalSize = totalSize;
            return args;
        }

        void IReference.OnRecyle()
        {
            DownloadedSize = 0;
            TotalSize = 0;
        }
    }
}
