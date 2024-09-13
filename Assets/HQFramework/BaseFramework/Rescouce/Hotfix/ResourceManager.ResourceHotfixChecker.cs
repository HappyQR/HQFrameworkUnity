using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private sealed class ResourceHotfixChecker
        {
            private static readonly byte hotfixTimeout = 5;

            private readonly ResourceManager resourceManager;

            public ResourceHotfixChecker(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
            }

            public async void CheckHotfix()
            {
                try
                {
                    using HttpClient client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(hotfixTimeout);
                    string remoteManifestJson = await client.GetStringAsync(resourceManager.config.hotfixManifestUrl);
                    resourceManager.remoteManifest = SerializeManager.JsonToObject<AssetModuleManifest>(remoteManifestJson);
                    HotfixCheckCompleteEventArgs args = CheckAssetsHotfix();
                    resourceManager.onHotfixCheckComplete?.Invoke(args);

                    // clear hotfix check
                    resourceManager.onHotfixCheckError = null;
                    resourceManager.onHotfixCheckComplete = null;
                }
                catch (Exception ex)
                {
                    HotfixCheckErrorEventArgs errorArgs = new HotfixCheckErrorEventArgs(ex.Message);
                    resourceManager.onHotfixCheckError?.Invoke(errorArgs);
                }
            }

            public HotfixCheckCompleteEventArgs CheckModuleHotfix(int moduleID)
            {
                AssetModuleInfo remoteModule = resourceManager.remoteManifest.moduleDic[moduleID];
                if (resourceManager.localManifest.moduleDic.ContainsKey(moduleID))
                {
                    AssetModuleInfo localModule = resourceManager.localManifest.moduleDic[moduleID];
                    List<AssetBundleInfo> bundleList = DiffModule(localModule, remoteModule);
                    if (bundleList.Count == 0)
                    {
                        HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(true, false, null, 0);
                        return args;
                    }
                    else
                    {
                        resourceManager.separateHotfixContent.Add(remoteModule, bundleList);
                        bool forceUpdate = localModule.currentPatchVersion < remoteModule.minimalSupportedPatchVersion;
                        string releaseNote = remoteModule.releaseNote;
                        int totalSize = 0;
                        for (int i = 0; i < bundleList.Count; i++)
                        {
                            totalSize += bundleList[i].size;
                        }
                        HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(false, forceUpdate, releaseNote, totalSize);
                        return args;
                    }
                }
                else
                {
                    List<AssetBundleInfo> bundleList = remoteModule.bundleDic.Values.ToList();
                    resourceManager.separateHotfixContent.Add(remoteModule, bundleList);
                    string releaseNote = remoteModule.releaseNote;
                    int totalSize = 0;
                    for (int i = 0; i < bundleList.Count; i++)
                    {
                        totalSize += bundleList[i].size;
                    }
                    HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(false, true, releaseNote, totalSize);
                    return args;
                }
            }

            private HotfixCheckCompleteEventArgs CheckAssetsHotfix()
            {
                if (!resourceManager.localManifest.isBuiltinManifest && resourceManager.localManifest.resourceVersion == resourceManager.remoteManifest.resourceVersion)
                {
                    HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(true, false, null, 0);
                    return args;
                }

                bool forceUpdate = resourceManager.localManifest.resourceVersion < resourceManager.remoteManifest.minimalSupportedVersion;
                string releaseNote = resourceManager.remoteManifest.releaseNote;
                resourceManager.necessaryHotfixContent = new Dictionary<AssetModuleInfo, List<AssetBundleInfo>>();
                foreach (AssetModuleInfo remoteModule in resourceManager.remoteManifest.moduleDic.Values)
                {
                    if (resourceManager.config.hotfixMode == AssetHotfixMode.SeparateHotfix && !remoteModule.isBuiltin)
                    {
                        continue;
                    }
                    if (!resourceManager.localManifest.moduleDic.ContainsKey(remoteModule.id))
                    {           
                        resourceManager.necessaryHotfixContent.Add(remoteModule, remoteModule.bundleDic.Values.ToList());
                    }
                    else
                    {
                        AssetModuleInfo localModule = resourceManager.localManifest.moduleDic[remoteModule.id];
                        List<AssetBundleInfo> bundleList = DiffModule(localModule, remoteModule);
                        if (bundleList.Count == 0)
                        {
                            continue;
                        }
                        resourceManager.necessaryHotfixContent.Add(remoteModule, bundleList);
                    }
                }
                bool isLatest = resourceManager.necessaryHotfixContent.Count == 0;
                int totalSize = 0;
                foreach (List<AssetBundleInfo> bundleList in resourceManager.necessaryHotfixContent.Values)
                {
                    for (int i = 0; i < bundleList.Count; i++)
                    {
                        totalSize += bundleList[i].size;
                    }
                }
                HotfixCheckCompleteEventArgs checkArgs = new HotfixCheckCompleteEventArgs(isLatest, forceUpdate, releaseNote, totalSize);
                return checkArgs;
            }

            private List<AssetBundleInfo> DiffModule(AssetModuleInfo localModule, AssetModuleInfo remoteModule)
            {
                List<AssetBundleInfo> bundleList = new List<AssetBundleInfo>();
                foreach (AssetBundleInfo remoteBundle in remoteModule.bundleDic.Values)
                {
                    if (!localModule.bundleDic.ContainsKey(remoteBundle.bundleName) ||
                        localModule.bundleDic[remoteBundle.bundleName].md5 != remoteBundle.md5)
                    {
                        bundleList.Add(remoteBundle);
                    }
                }
                return bundleList;
            }
        }
    }
}
