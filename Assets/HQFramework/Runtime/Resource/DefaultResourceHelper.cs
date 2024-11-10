using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HQFramework.Coroutine;
using HQFramework.Resource;
using UnityEngine;
using UnityEngine.Networking;
using UnityObject = UnityEngine.Object;

namespace HQFramework.Runtime
{
    internal class DefaultResourceHelper : IResourceHelper
    {
        private static readonly string manifestFileName = "HQAssetManifest.json";

        private string localManifestFilePath;

        private Dictionary<int, UnityObject> instantiateDic = new Dictionary<int, UnityObject>();

        public int LauncherHotfixID
        {
            get;
            set;
        }

        public HQHotfixMode HotfixMode
        {
            get;
            set;
        }

        public string AssetsPersistentDir
        {
            get;
            set;
        }

        public string AssetsBuiltinDir
        {
            get;
            set;
        }

        public string HotfixManifestUrl
        {
            get;
            set;
        }

        public void DecompressBuiltinAssets(Action callback)
        {
            HQFrameworkEngine.GetModule<ICoroutineManager>().StartCoroutine(DecompressBuiltinAssetsInternal(callback));
        }

        public async void LoadAssetManifest(Action<ManifestLoadCompleteEventArgs> callback)
        {
            if (string.IsNullOrEmpty(localManifestFilePath))
            {
                localManifestFilePath = Path.Combine(AssetsPersistentDir, manifestFileName);
            }
            if (File.Exists(localManifestFilePath))
            {
                string localManifestJsonStr = await File.ReadAllTextAsync(localManifestFilePath);
                HQAssetManifest localManifest = SerializeManager.JsonToObject<HQAssetManifest>(localManifestJsonStr);
                ManifestLoadCompleteEventArgs args = new ManifestLoadCompleteEventArgs(localManifest);
                callback?.Invoke(args);
            }
            else
            {
                string lcoalManifestUrl = "file://" + Path.Combine(AssetsBuiltinDir, manifestFileName);
                UnityWebRequest localManifestRequest = UnityWebRequest.Get(lcoalManifestUrl);
                UnityWebRequestAsyncOperation requestAsyncOperation = localManifestRequest.SendWebRequest();
                requestAsyncOperation.completed += (op) =>
                {
                    string localManifestJsonStr = localManifestRequest.downloadHandler.text;
                    HQAssetManifest localManifest = SerializeManager.JsonToObject<HQAssetManifest>(localManifestJsonStr);
                    ManifestLoadCompleteEventArgs args = new ManifestLoadCompleteEventArgs(localManifest);
                    callback?.Invoke(args);
                    localManifestRequest.Dispose();
                };
            }
        }

        public void OverrideLocalManifest(HQAssetManifest localManifest)
        {
            string manifestJson = SerializeManager.ObjectToJson(localManifest);
            File.WriteAllText(localManifestFilePath, manifestJson);
        }

        public string GetBundleRelatedPath(HQAssetBundleConfig bundleInfo)
        {
            return Path.Combine(AssetsPersistentDir, bundleInfo.moduleID.ToString(), bundleInfo.bundleName);
        }

        public void DeleteAssetModule(HQAssetModuleConfig module)
        {
            string moduleDir = Path.Combine(AssetsPersistentDir, module.id.ToString());
            if (Directory.Exists(moduleDir))
            {
                Directory.Delete(moduleDir, true);
            }
        }

        public void LoadAsset(object bundle, string assetPath, Type assetType, Action<object> callback)
        {
            AssetBundle assetBundle = (AssetBundle)bundle;
            AssetBundleRequest request = assetBundle.LoadAssetAsync(assetPath, assetType);
            request.completed += (asyncOperation) =>
            {
                callback?.Invoke(request.asset);
            };
        }

        public void LoadAsset(object bundle, string assetPath, Action<object> callback)
        {
            AssetBundle assetBundle = (AssetBundle)bundle;
            AssetBundleRequest request = assetBundle.LoadAssetAsync(assetPath);
            request.completed += (asyncOperation) =>
            {
                callback?.Invoke(request.asset);
            };
        }

        public void LoadAssetBundle(string bundlePath, Action<object> callback)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
            request.completed += (asyncOperation) =>
            {
                callback?.Invoke(request.assetBundle);
            };
        }

        public bool IsIndividualAsset(object asset)
        {
            return asset is not GameObject;
        }

        public object InstantiateAsset(object asset)
        {
            UnityObject instance = UnityObject.Instantiate(asset as UnityObject);
            instantiateDic.Add(instance.GetInstanceID(), instance);
            return instance;
        }

        public void UnloadAsset(object asset)
        {
            UnityObject target = asset as UnityObject;
            int instanceID = target.GetInstanceID();
            if (instantiateDic.ContainsKey(instanceID))
            {
                UnityObject.Destroy(target);
                instantiateDic.Remove(instanceID);
            }
            else if (target is not GameObject)
            {
                Resources.UnloadAsset(target);
            }
        }

        public void UnloadBundle(object bundle)
        {
            (bundle as AssetBundle).Unload(true);
        }

        private IEnumerator DecompressBuiltinAssetsInternal(Action onComplete)
        {
            string localManifestFilePath = Path.Combine(AssetsPersistentDir, manifestFileName);
            if (File.Exists(localManifestFilePath))
            {
                onComplete?.Invoke();
                yield break;
            }

            string lcoalManifestUrl = "file://" + Path.Combine(AssetsBuiltinDir, manifestFileName);
            using UnityWebRequest localManifestRequest = UnityWebRequest.Get(lcoalManifestUrl);
            localManifestRequest.SendWebRequest();
            while (!localManifestRequest.isDone)
            {
                yield return null;
            }
            string manifestJson = localManifestRequest.downloadHandler.text;
            HQAssetManifest localManifest = SerializeManager.JsonToObject<HQAssetManifest>(manifestJson);
            foreach (var module in localManifest.moduleDic.Values)
            {
                string moudleDir = Path.Combine(AssetsPersistentDir, module.id.ToString());
                string moduleUrl = "file://" + Path.Combine(AssetsBuiltinDir, module.id.ToString());
                if (!Directory.Exists(moudleDir))
                {
                    Directory.CreateDirectory(moudleDir);
                }
                foreach (var bundle in module.bundleDic.Values)
                {
                    string bundlePath = Path.Combine(moudleDir, bundle.bundleName);
                    string bundleUrl = Path.Combine(moduleUrl, bundle.bundleName);
                    using UnityWebRequest bundleRequest = UnityWebRequest.Get(bundleUrl);
                    bundleRequest.SendWebRequest();
                    while (!bundleRequest.isDone)
                    {
                        yield return null;
                    }
                    File.WriteAllBytes(bundlePath, bundleRequest.downloadHandler.data);
                }
            }
            File.WriteAllText(localManifestFilePath, manifestJson);
            onComplete?.Invoke();
        }
    }
}
