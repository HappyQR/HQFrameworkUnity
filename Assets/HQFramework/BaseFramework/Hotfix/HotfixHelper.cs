using System;
using System.Collections.Generic;
using HQFramework.Resource;

namespace HQFramework.Hotfix
{
    internal abstract class HotfixHelper
    {
        protected string hotfixUrl;
        protected string assetPersistentDir;
        protected List<HotfixPatch> patchList;

        public event Action<HotfixErrorEventArgs> onHotfixError;
        public event Action<HotfixUpdateEventArgs> onHotfixUpdate;
        public event Action onHotfixDone;

        public HotfixHelper(string hotfixUrl, string assetPersistentDir)
        {
            this.hotfixUrl = hotfixUrl;
            this.assetPersistentDir = assetPersistentDir;
        }

        public abstract HotfixCheckEventArgs CheckManifestUpdate(AssetModuleManifest localManifest, AssetModuleManifest remoteManifest);
    }
}
