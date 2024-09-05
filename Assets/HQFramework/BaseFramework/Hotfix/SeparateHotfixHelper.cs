using HQFramework.Resource;

namespace HQFramework.Hotfix
{
    internal sealed class SeparateHotfixHelper : HotfixHelper
    {
        public SeparateHotfixHelper(string hotfixUrl, string assetPersistentDir) : base(hotfixUrl, assetPersistentDir)
        {
        }

        public override HotfixCheckEventArgs CheckManifestUpdate(AssetModuleManifest localManifest, AssetModuleManifest remoteManifest)
        {
            throw new System.NotImplementedException();
        }
    }
}
