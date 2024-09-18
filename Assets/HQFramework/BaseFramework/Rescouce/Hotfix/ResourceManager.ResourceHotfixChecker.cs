using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HQFramework.Coroutine;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private sealed class ResourceHotfixChecker
        {
            private static readonly byte hotfixTimeout = 5;

            private readonly ResourceManager resourceManager;
            private readonly ICoroutineManager coroutineManager;

            private Dictionary<int, Action<HotfixCheckErrorEventArgs>> errorEventDic;
            private Dictionary<int, Action<HotfixCheckCompleteEventArgs>> completeEventDic;

            public ResourceHotfixChecker(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
                coroutineManager = HQFrameworkEngine.GetModule<ICoroutineManager>();
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
                coroutineManager.StartCoroutine(ModuleHotfixCheckInternal(moduleID));
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
                if (resourceManager.localManifest == null)
                {
                    resourceManager.localManifest = await resourceManager.resourceHelper.LoadAssetManifestAsync();
                }
                try
                {
                    using HttpClient client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(hotfixTimeout);
                    string remoteManifestJson = await client.GetStringAsync(resourceManager.config.hotfixManifestUrl);
                    resourceManager.remoteManifest = SerializeManager.JsonToObject<AssetModuleManifest>(remoteManifestJson);
                    HotfixCheckCompleteEventArgs args = LaunchAssetsHotfix();
                    InvokeCompleteEvent(hotfixID, args);
                }
                catch (Exception ex)
                {
                    HotfixCheckErrorEventArgs errorArgs = new HotfixCheckErrorEventArgs(hotfixID, ex.Message);
                    InvokeErrorEvent(hotfixID, errorArgs);
                }
            }

            private IEnumerator ModuleHotfixCheckInternal(int moduleID)
            {
                if (!resourceManager.remoteManifest.moduleDic.ContainsKey(moduleID))
                {
                    HotfixCheckErrorEventArgs args = new HotfixCheckErrorEventArgs(moduleID, "Module doesn't exists");
                    InvokeErrorEvent(moduleID, args);
                    yield break;
                }
                AssetModuleInfo remoteModule = resourceManager.remoteManifest.moduleDic[moduleID];
                if (resourceManager.localManifest.moduleDic.ContainsKey(moduleID))
                {
                    AssetModuleInfo localModule = resourceManager.localManifest.moduleDic[moduleID];
                    List<AssetBundleInfo> bundleList = DiffModule(localModule, remoteModule);
                    if (bundleList.Count == 0)
                    {
                        HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(moduleID, true, false, null, 0);
                        InvokeCompleteEvent(moduleID, args);
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
                        HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(moduleID, false, forceUpdate, releaseNote, totalSize);
                        InvokeCompleteEvent(moduleID, args);
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
                    HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(moduleID, false, true, releaseNote, totalSize);
                    InvokeCompleteEvent(moduleID, args);
                }
            }

            private HotfixCheckCompleteEventArgs LaunchAssetsHotfix()
            {
                int hotfixID = resourceManager.resourceHelper.LauncherHotfixID;
                if (!resourceManager.localManifest.isBuiltinManifest && resourceManager.localManifest.resourceVersion == resourceManager.remoteManifest.resourceVersion)
                {
                    HotfixCheckCompleteEventArgs args = new HotfixCheckCompleteEventArgs(hotfixID, true, false, null, 0);
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
                HotfixCheckCompleteEventArgs checkArgs = new HotfixCheckCompleteEventArgs(hotfixID, isLatest, forceUpdate, releaseNote, totalSize);
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
