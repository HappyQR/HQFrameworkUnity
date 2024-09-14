namespace HQFramework.Download
{
    internal partial class DownloadManager
    {
        private enum DownloadResult : byte
        {
            None,
            Complete,
            Error,
            Canceled
        }
    }
}
