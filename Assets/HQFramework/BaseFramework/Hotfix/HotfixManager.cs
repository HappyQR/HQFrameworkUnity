using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using HQFramework.Download;
using HQFramework.Resource;

namespace HQFramework.Hotfix
{
    internal sealed partial class HotfixManager : HQModuleBase, IHotfixManager
    {
        private static readonly byte hotfixTimeout = 10;
        private static readonly int hotfixDownloadGroupID = 1;

        private ResourceConfig config;
        private AssetModuleManifest localManifest;
        private AssetModuleManifest remoteManifest;
        private List<HotfixPatch> patchList;
        private Dictionary<int, HotfixDownloadItem> downloadDic;
        private List<HotfixDownloadItem> failedDownloadList;
        private float totalSize;
        private float downloadedSize;

        public override byte Priority => byte.MaxValue;

        public event Action<HotfixCheckEventArgs> onHotfixCheckDone;
        public event Action<HotfixCheckErrorEventArgs> onHotfixCheckError;
        public event Action<HotfixErrorEventArgs> onHotfixError;
        public event Action<HotfixUpdateEventArgs> onHotfixUpdate;
        public event Action onHotfixDone;

        public void InitHotfixModule(ResourceConfig config, AssetModuleManifest localManifest)
        {
            this.config = config;
            this.localManifest = localManifest;
        }

        public void StartHotfix()
        {
            IDownloadManager downloadManager = HQFrameworkEngine.GetModule<IDownloadManager>();
            downloadDic = new Dictionary<int, HotfixDownloadItem>();
            failedDownloadList = new List<HotfixDownloadItem>();
            string hotfixUrlRoot = Path.Combine(config.hotfixUrl, remoteManifest.resourceVersion.ToString());
            for (int i = 0; i < patchList.Count; i++)
            {
                AssetModuleInfo module = patchList[i].module;
                string moduleUrlRoot = Path.Combine(hotfixUrlRoot, module.moduleName, module.currentPatchVersion.ToString());
                string moduleLocalDir = Path.Combine(config.assetPersistentDir, module.moduleName);
                if (!Directory.Exists(moduleLocalDir))
                {
                    Directory.CreateDirectory(moduleLocalDir);
                }
                for (int j = 0; j < patchList[i].bundleList.Count; j++)
                {
                    AssetBundleInfo bundle = patchList[i].bundleList[j];
                    string bundleUrl = Path.Combine(moduleUrlRoot, bundle.bundleName);
                    string bundlePath = Path.Combine(moduleLocalDir, bundle.bundleName);
                    int downloadID = downloadManager.AddDownload(bundleUrl, bundlePath, false, hotfixDownloadGroupID, 0);
                    downloadDic.Add(downloadID, new HotfixDownloadItem(bundleUrl, bundlePath, bundle));
                }
            }
        }

        public async void StartHotfixCheck()
        {
            try
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(hotfixTimeout);
                string jsonStr = await client.GetStringAsync(config.hotfixManifestUrl);
                remoteManifest = SerializeManager.JsonToObject<AssetModuleManifest>(jsonStr);
                Type helperType = Type.GetType($"HQFramework.Hotfix.{config.hotfixMode}Checker");
                IHotfixChecker checker = Activator.CreateInstance(helperType) as IHotfixChecker;
                HotfixCheckEventArgs args = checker.CheckManifestUpdate(localManifest, remoteManifest);
                patchList = args.patchList;
                totalSize = args.totalSize;
                onHotfixCheckDone?.Invoke(args);
            }
            catch (Exception ex)
            {
                HotfixCheckErrorEventArgs args = new HotfixCheckErrorEventArgs(ex.Message);
                onHotfixCheckError?.Invoke(args);
            }
        }

        private void DeleteObsoleteAssets()
        {

        }

        private void OnDownloadBundleError(DownloadErrorEventArgs args)
        {
            
        }

        private void OnDownloadBundleDone(TaskInfo taskInfo)
        {
            HotfixDownloadItem item = downloadDic[taskInfo.id];
            // do the hash check.
        }

        private void OnDownloadBundleUpdate(DownloadUpdateEventArgs args)
        {
            downloadedSize += args.DeltaSize;
        }

        private void ClearHotfix()
        {

        }

        protected override void OnShutdown()
        {
            
        }
    }
}
