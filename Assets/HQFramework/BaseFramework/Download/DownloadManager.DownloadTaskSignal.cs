namespace HQFramework.Download
{
    internal partial class DownloadManager
    {
        private class DownloadTaskSignal : IReference
        {
            public bool Succeeded { get; private set; }
            public string ErrorMessage { get; private set; }
            public int DownloadedSize { get; private set; }
            public int TotalSize { get; private set; }

            public static DownloadTaskSignal Create(bool succeeded, string errorMessage, int downloadedSize, int totalSize)
            {
                DownloadTaskSignal signal = ReferencePool.Spawn<DownloadTaskSignal>();
                signal.Succeeded = succeeded;
                signal.ErrorMessage = errorMessage;
                signal.DownloadedSize = downloadedSize;
                signal.TotalSize = totalSize;
                return signal;
            }

            void IReference.OnRecyle()
            {
                Succeeded = false;
                ErrorMessage = null;
                DownloadedSize = 0;
            }
        }
    }
}
