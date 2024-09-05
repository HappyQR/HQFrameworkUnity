using System.Collections.Generic;
using System.IO;
using System.Text;
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
            patchList = new List<HotfixPatch>();
            bool forceUpdate = false;
            foreach (AssetModuleInfo remoteModule in remoteManifest.moduleDic.Values)
            {
                // separate hotfix only check the built-in module.
                if (!remoteModule.isBuiltin)
                {
                    continue;
                }

                if (!localManifest.moduleDic.ContainsKey(remoteModule.id))
                {
                    forceUpdate = true;
                    patchList.Add(new HotfixPatch(remoteModule, remoteModule.bundleDic.Values));
                    continue;
                }

                AssetModuleInfo localModule = localManifest.moduleDic[remoteModule.id];
                if (localModule.currentPatchVersion == remoteModule.currentPatchVersion)
                {
                    continue;
                }
                forceUpdate = localModule.currentPatchVersion < remoteModule.minimalSupportedPatchVersion;
                List<AssetBundleInfo> bundleList = new List<AssetBundleInfo>();
                foreach (AssetBundleInfo remoteBundle in remoteModule.bundleDic.Values)
                {
                    if (!localModule.bundleDic.ContainsKey(remoteBundle.bundleName) ||
                        localModule.bundleDic[remoteBundle.bundleName].md5 != remoteBundle.md5)
                    {
                        bundleList.Add(remoteBundle);
                    }
                }
                if (bundleList.Count > 0)
                {
                    patchList.Add(new HotfixPatch(remoteModule, bundleList));
                }
            }

            bool isLatest = patchList.Count == 0;
            StringBuilder releaseNote = new StringBuilder();
            int totalSize = 0;
            for (int i = 0; i < patchList.Count; i++)
            {
                releaseNote.Append(patchList[i].module.releaseNote);
                releaseNote.Append('\n');
                for (int j = 0; j < patchList[i].bundleList.Count; j++)
                {
                    totalSize += patchList[i].bundleList[j].size;
                }
            }

            if (forceUpdate)
            {
                // delete the obsolete modules
                foreach (AssetModuleInfo localModule in localManifest.moduleDic.Values)
                {
                    if (!remoteManifest.moduleDic.ContainsKey(localModule.id))
                    {
                        string moduleDir = Path.Combine(assetPersistentDir, localModule.moduleName);
                        if (Directory.Exists(moduleDir))
                        {
                            Directory.Delete(moduleDir, true);
                        }
                    }
                }
            }

            HotfixCheckEventArgs checkEventArgs = new HotfixCheckEventArgs(isLatest, forceUpdate, releaseNote.ToString(), totalSize);
            return checkEventArgs;
        }
    }
}
