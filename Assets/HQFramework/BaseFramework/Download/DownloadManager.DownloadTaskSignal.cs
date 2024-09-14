namespace HQFramework.Download
{
    internal partial class DownloadManager
    {
        private class DownloadTaskSignal : IReference
        {
            public DownloadResult Result { get; private set; }
            public string ErrorMessage { get; private set; }
            public int DownloadedSize { get; private set; }
            public int TotalSize { get; private set; }

            public static DownloadTaskSignal Create(DownloadResult result, string errorMessage, int downloadedSize, int totalSize)
            {
                DownloadTaskSignal signal = ReferencePool.Spawn<DownloadTaskSignal>();
                signal.Result = result;
                signal.ErrorMessage = errorMessage;
                signal.DownloadedSize = downloadedSize;
                signal.TotalSize = totalSize;
                return signal;
            }

            void IReference.OnRecyle()
            {
                Result = DownloadResult.None;
                ErrorMessage = null;
                DownloadedSize = 0;
            }
        }
    }
}
