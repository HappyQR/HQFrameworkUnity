using System;
using System.Net;
using System.Net.Http;
using HQFramework.Resource;

namespace HQFramework.Hotfix
{
    internal sealed partial class HotfixManager : HQModuleBase, IHotfixManager
    {
        private static readonly byte hotfixTimeout = 10;

        private HotfixHelper hotfixHelper;
        private ResourceConfig config;
        private AssetModuleManifest localManifest;
        private AssetModuleManifest remoteManifest;

        public override byte Priority => byte.MaxValue;

        public event Action<HotfixCheckEventArgs> onHotfixCheckDone;
        public event Action<HotfixCheckErrorEventArgs> onHotfixCheckError;
        public event Action<HotfixErrorEventArgs> onHotfixError;
        public event Action<HotfixUpdateEventArgs> onHotfixUpdate;
        public event Action onHotfixDone;

        public void InitHotfixModule(ResourceConfig config, AssetModuleManifest localManifest)
        {
            this.config = config;
            Type helperType = Type.GetType($"HQFramework.Hotfix.{config.hotfixMode}Hepler");
            hotfixHelper = Activator.CreateInstance(helperType, config.hotfixUrl, config.assetPersistentDir) as HotfixHelper;
        }

        public void StartHotfix()
        {
            
        }

        public async void StartHotfixCheck()
        {
            try
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(hotfixTimeout);
                string jsonStr = await client.GetStringAsync(config.hotfixManifestUrl);
                remoteManifest = SerializeManager.JsonToObject<AssetModuleManifest>(jsonStr);

                HotfixCheckEventArgs args = hotfixHelper.CheckManifestUpdate(localManifest, remoteManifest);
                onHotfixCheckDone?.Invoke(args);
            }
            catch (Exception ex)
            {
                HotfixCheckErrorEventArgs args = new HotfixCheckErrorEventArgs(ex.Message);
                onHotfixCheckError?.Invoke(args);
            }
        }

        protected override void OnShutdown()
        {
            
        }
    }
}
