using HQFramework.Resource;

namespace HQFramework.Hotfix
{
    internal interface IHotfixChecker
    {
        public HotfixCheckEventArgs CheckManifestUpdate(AssetModuleManifest localManifest, AssetModuleManifest remoteManifest);
    }
}
