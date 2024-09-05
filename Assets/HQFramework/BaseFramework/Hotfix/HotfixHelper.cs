using HQFramework.Resource;

namespace HQFramework.Hotfix
{
    internal abstract class HotfixHelper
    {
        protected string hotfixUrl;
        protected string assetPersistentDir;

        public HotfixHelper(string hotfixUrl, string assetPersistentDir)
        {
            this.hotfixUrl = hotfixUrl;
            this.assetPersistentDir = assetPersistentDir;
        }

        public abstract HotfixCheckEventArgs CheckManifestUpdate(AssetModuleManifest localManifest, AssetModuleManifest remoteManifest);
    }
}
