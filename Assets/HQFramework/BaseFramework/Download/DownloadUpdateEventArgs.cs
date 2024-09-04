namespace HQFramework.Download
{
    public class DownloadUpdateEventArgs : IReference
    {
        public int ID { get; private set; }
        public int GroupID { get; private set; }
        public string Url { get; private set; }
        public string FilePath { get; private set; }
        public int DeltaSize { get; private set; }
        public int DownloadedSize { get; private set; }
        public int TotalSize { get; private set; }

        public static DownloadUpdateEventArgs Create(int id, int groupID, string url, string filePath, int deltaSize, int downloadedSize, int totalSize)
        {
            DownloadUpdateEventArgs args = ReferencePool.Spawn<DownloadUpdateEventArgs>();
            args.ID = id;
            args.GroupID = groupID;
            args.Url = url;
            args.FilePath = filePath;
            args.DeltaSize = deltaSize;
            args.DownloadedSize = downloadedSize;
            args.TotalSize = totalSize;

            return args;
        }

        void IReference.OnRecyle()
        {
            ID = -1;
            GroupID = -1;
            Url = null;
            FilePath = null;
            DeltaSize = DownloadedSize = TotalSize = 0;
        }
    }
}
