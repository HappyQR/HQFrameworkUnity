namespace HQFramework.Download
{
    public class DownloadHashCheckEventArgs : IReference
    {
        public int ID { get; private set; }
        public int GroupID { get; private set; }
        public string Url { get; private set; }
        public string FilePath { get; private set; }
        public string TargetHash { get; private set; }
        public string LocalHash { get; private set; }
        public bool Result { get; private set; }

        public static DownloadHashCheckEventArgs Create(int id, int groupID, string url, string filePath, string targetHash, string localHash, bool result)
        {
            DownloadHashCheckEventArgs args = ReferencePool.Spawn<DownloadHashCheckEventArgs>();
            args.ID = id;
            args.GroupID = groupID;
            args.Url = url;
            args.FilePath = filePath;
            args.TargetHash = targetHash;
            args.LocalHash = localHash;
            args.Result = result;
            return args;
        }

        void IReference.OnRecyle()
        {
            ID = -1;
            GroupID = -1;
            Url = null;
            FilePath = null;
            TargetHash = null;
            LocalHash = null;
            Result = false;
        }
    }
}
