using HQFramework.Resource;

namespace HQFramework.Hotfix
{
    internal partial class HotfixManager
    {
        internal class HotfixDownloadItem
        {
            public readonly string url;
            public readonly string filePath;
            public readonly AssetBundleInfo bundle;

            public HotfixDownloadItem(string url, string filePath, AssetBundleInfo bundle)
            {
                this.url = url;
                this.filePath = filePath;
                this.bundle = bundle;
            }
        }
    }
}
