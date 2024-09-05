using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;

namespace HQFramework.Hotfix
{
    internal sealed class PreHotfixHelper : HotfixHelper
    {
        public PreHotfixHelper(string hotfixUrl, string assetPersistentDir) : base(hotfixUrl, assetPersistentDir)
        {
        }

        public override HotfixCheckEventArgs CheckManifestUpdate(AssetModuleManifest localManifest, AssetModuleManifest remoteManifest)
        {
            if (!localManifest.isBuiltinManifest && localManifest.resourceVersion == remoteManifest.resourceVersion)
            {
                HotfixCheckEventArgs args = new HotfixCheckEventArgs(true, false, null, 0);
                return args;
            }
            
            patchList = new List<HotfixPatch>();
            foreach (AssetModuleInfo remoteModule in remoteManifest.moduleDic.Values)
            {
                if (!localManifest.moduleDic.ContainsKey(remoteModule.id))
                {
                    patchList.Add(new HotfixPatch(remoteModule, remoteModule.bundleDic.Values));
                    continue;
                }

                List<AssetBundleInfo> bundleList = new List<AssetBundleInfo>();
                AssetModuleInfo localModule = localManifest.moduleDic[remoteModule.id];
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
            bool forceUpdate = localManifest.resourceVersion < remoteManifest.minimalSupportedVersion;
            string releaseNote = remoteManifest.releaseNote;
            int totalSize = 0;
            for (int i = 0; i < patchList.Count; i++)
            {
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

            HotfixCheckEventArgs checkEventArgs = new HotfixCheckEventArgs(isLatest, forceUpdate, releaseNote, totalSize);
            return checkEventArgs;
        }
    }
}
