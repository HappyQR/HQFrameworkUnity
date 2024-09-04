namespace HQFramework.Download
{
    public class DownloadErrorEventArgs : IReference
    {
        public int ID { get; private set; }
        public int GroupID { get; private set; }
        public string Url { get; private set; }
        public string FilePath { get; private set; }
        public string ErrorMsg { get; private set; }

        public static DownloadErrorEventArgs Create(int id, int groupID, string url, string filePath, string errorMsg)
        {
            DownloadErrorEventArgs args = ReferencePool.Spawn<DownloadErrorEventArgs>();
            args.ID = id;
            args.GroupID = groupID;
            args.Url = url;
            args.FilePath = filePath;
            args.ErrorMsg = errorMsg;
            return args;
        }

        void IReference.OnRecyle()
        {
            ID = -1;
            GroupID = -1;
            Url = null;
            FilePath = null;
            ErrorMsg = null;
        }
    }
}
