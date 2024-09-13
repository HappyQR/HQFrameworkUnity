using System.Collections.Generic;
using HQFramework.Download;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private sealed class ResourceDownloader
        {
            private readonly ResourceManager resourceManager;
            private readonly IDownloadManager downloadManager;
            private readonly int hotfixDownloadGroupID;

            public ResourceDownloader(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
                this.downloadManager = HQFrameworkEngine.GetModule<IDownloadManager>();
                this.hotfixDownloadGroupID = resourceManager.resourceHelper.HotfixDownloadGroupID;
            }

            public void StartHotfix()
            {
                
            }

            public void OnUpdate()
            {
                
            }

            public int DownloadModule(AssetModuleInfo module, List<AssetBundleInfo> bundleList)
            {
                return 0;
            }

            private void ClearDownload()
            {
                
            }
        }
    }
}
