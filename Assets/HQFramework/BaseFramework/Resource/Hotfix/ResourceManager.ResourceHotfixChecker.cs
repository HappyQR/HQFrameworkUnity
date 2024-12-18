using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private sealed class ResourceHotfixChecker
        {
            private static readonly byte hotfixTimeout = 5;

            private readonly ResourceManager resourceManager;

            private Dictionary<int, Action<HotfixCheckErrorEventArgs>> errorEventDic;
            private Dictionary<int, Action<HotfixCheckCompleteEventArgs>> completeEventDic;

            public ResourceHotfixChecker(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
                errorEventDic = new Dictionary<int, Action<HotfixCheckErrorEventArgs>>();
                completeEventDic = new Dictionary<int, Action<HotfixCheckCompleteEventArgs>>();
            }

            public int LaunchHotfix()
            {
                LaunchHotfixInternal();
                return resourceManager.resourceHelper.LauncherHotfixID;
            }

            public int ModuleHotfixCheck(int moduleID)
            {
                ModuleHotfixCheckInternal(moduleID);
                return moduleID;
            }

            public void AddHotfixCheckErrorEvent(int hotfixID, Action<HotfixCheckErrorEventArgs> onHotfixCheckError)
            {
                if (errorEventDic.ContainsKey(hotfixID))
                {
                    errorEventDic[hotfixID] += onHotfixCheckError;
                }
                else
                {
                    errorEventDic.Add(hotfixID, onHotfixCheckError);
                }
            }

            public void AddHotfixCheckCompleteEvent(int hotfixID, Action<HotfixCheckCompleteEventArgs> onHotfixCheckComplete)
            {
                if (completeEventDic.ContainsKey(hotfixID))
                {
                    completeEventDic[hotfixID] += onHotfixCheckComplete;
                }
                else
                {
                    completeEventDic.Add(hotfixID, onHotfixCheckComplete);                    
                }
            }

            private async void LaunchHotfixInternal()
            {
                int hotfixID = resourceManager.resourceHelper.LauncherHotfixID;
                try
                {
                    using HttpClient client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(hotfixTimeout);
                    string remoteManifestJson = await client.GetStringAsync(resourceManager.resourceHelper.HotfixManifestUrl);
                    resourceManager.remoteManifest = SerializeManager.JsonToObject<HQAssetManifest>(remoteManifestJson);
                    while (resourceManager.localManifest == null)
                    {
                        await Task.Yield();
                    }
                    HotfixCheckCompleteEventArgs args = LaunchAssetsHotfix();
                    InvokeCompleteEvent(hotfixID, args);
                }
                catch (Exception ex)
                {
                    HotfixCheckErrorEventArgs errorArgs = new HotfixCheckErrorEventArgs(hotfixID, ex.Message);
                    InvokeErrorEvent(hotfixID, errorArgs);
                }
            }

            private async void ModuleHotfixCheckInternal(int moduleID)
            {
                await Task.Yield();
                if (!resourceManager.remoteManifest.moduleDic.ContainsKey(moduleID))
                {
                    HotfixCheckErrorEventArgs args = new HotfixCheckErrorEventArgs(moduleID, "Module doesn't exists");
                    InvokeErrorEvent(moduleID, args);
                }
                HQAssetModuleConfig remoteModule = resourceManager.remoteManifest.moduleDic[moduleID];
                if (resourceManager.localManifest.moduleDic.ContainsKey(moduleID))
                {
                    HQAssetModuleConfig localModule = resourceManager.localManifest.moduleDic[moduleID];
                    List<HQAssetBundleConfig> bundleList = DiffModule(localModule, remoteModule);
                    if (bundleList == null || bundleList.Count == 0)
                    {
                        HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(moduleID, true, false, null, 0);
                        InvokeCompleteEvent(moduleID, args);
                    }
                    else
                    {
                        resourceManager.separateHotfixContent.Add(remoteModule, bundleList);
                        bool forceUpdate = localModule.currentPatchVersion > remoteModule.currentPatchVersion || localModule.currentPatchVersion < remoteModule.minimalSupportedPatchVersion;
                        string releaseNote = remoteModule.releaseNote;
                        int totalSize = 0;
                        for (int i = 0; i < bundleList.Count; i++)
                        {
                            totalSize += bundleList[i].size;
                        }
                        HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(moduleID, false, forceUpdate, releaseNote, totalSize);
                        InvokeCompleteEvent(moduleID, args);
                    }
                }
                else
                {
                    List<HQAssetBundleConfig> bundleList = remoteModule.bundleDic.Values.ToList();
                    resourceManager.separateHotfixContent.Add(remoteModule, bundleList);
                    string releaseNote = remoteModule.releaseNote;
                    int totalSize = 0;
                    for (int i = 0; i < bundleList.Count; i++)
                    {
                        totalSize += bundleList[i].size;
                    }
                    HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(moduleID, false, true, releaseNote, totalSize);
                    InvokeCompleteEvent(moduleID, args);
                }
            }

            private HotfixCheckCompleteEventArgs LaunchAssetsHotfix()
            {
                int hotfixID = resourceManager.resourceHelper.LauncherHotfixID;
                if (!resourceManager.localManifest.isBuiltinManifest && resourceManager.localManifest.versionCode == resourceManager.remoteManifest.versionCode)
                {
                    HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(hotfixID, true, false, null, 0);
                    return args;
                }

                bool forceUpdate = resourceManager.localManifest.versionCode > resourceManager.remoteManifest.versionCode || resourceManager.localManifest.versionCode < resourceManager.remoteManifest.minimalSupportedVersionCode;
                string releaseNote = resourceManager.remoteManifest.releaseNote;
                resourceManager.necessaryHotfixContent = new Dictionary<HQAssetModuleConfig, List<HQAssetBundleConfig>>();
                foreach (HQAssetModuleConfig remoteModule in resourceManager.remoteManifest.moduleDic.Values)
                {
                    if (resourceManager.resourceHelper.HotfixMode == HQHotfixMode.SeparateHotfix && !remoteModule.isBuiltin)
                    {
                        continue;
                    }
                    if (!resourceManager.localManifest.moduleDic.ContainsKey(remoteModule.id))
                    {           
                        resourceManager.necessaryHotfixContent.Add(remoteModule, remoteModule.bundleDic.Values.ToList());
                    }
                    else
                    {
                        HQAssetModuleConfig localModule = resourceManager.localManifest.moduleDic[remoteModule.id];
                        List<HQAssetBundleConfig> bundleList = DiffModule(localModule, remoteModule);
                        if (bundleList == null || bundleList.Count == 0)
                        {
                            continue;
                        }
                        resourceManager.necessaryHotfixContent.Add(remoteModule, bundleList);
                    }
                }
                bool isLatest = resourceManager.necessaryHotfixContent.Count == 0;
                int totalSize = 0;
                foreach (List<HQAssetBundleConfig> bundleList in resourceManager.necessaryHotfixContent.Values)
                {
                    for (int i = 0; i < bundleList.Count; i++)
                    {
                        totalSize += bundleList[i].size;
                    }
                }
                HotfixCheckCompleteEventArgs checkArgs = new HotfixCheckCompleteEventArgs(hotfixID, isLatest, forceUpdate, releaseNote, totalSize);
                return checkArgs;
            }

            private List<HQAssetBundleConfig> DiffModule(HQAssetModuleConfig localModule, HQAssetModuleConfig remoteModule)
            {
                if (localModule.currentPatchVersion == remoteModule.currentPatchVersion)
                {
                    return null;
                }
                
                List<HQAssetBundleConfig> bundleList = new List<HQAssetBundleConfig>();
                foreach (HQAssetBundleConfig remoteBundle in remoteModule.bundleDic.Values)
                {
                    if (!localModule.bundleDic.ContainsKey(remoteBundle.crc) ||
                        localModule.bundleDic[remoteBundle.crc].md5 != remoteBundle.md5)
                    {
                        bundleList.Add(remoteBundle);
                    }
                }
                return bundleList;
            }

            private void InvokeCompleteEvent(int hotfixID, HotfixCheckCompleteEventArgs args)
            {
                if (completeEventDic.ContainsKey(hotfixID))
                {
                    completeEventDic[hotfixID]?.Invoke(args);
                    completeEventDic.Remove(hotfixID);
                    errorEventDic.Remove(hotfixID);
                }
            }

            private void InvokeErrorEvent(int hotfixID, HotfixCheckErrorEventArgs args)
            {
                if (errorEventDic.ContainsKey(hotfixID))
                {
                    errorEventDic[hotfixID]?.Invoke(args);
                    errorEventDic.Remove(hotfixID);
                    completeEventDic.Remove(hotfixID);
                }
            }
        }
    }
}
