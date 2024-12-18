using System;
using System.Collections;
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
            return Path.Combine(bundleInfo.moduleID.ToString(), bundleInfo.bundleName);
        }

        public void DeleteAssetModule(HQAssetModuleConfig module)
        {
            string moduleDir = Path.Combine(AssetsPersistentDir, module.id.ToString());
            if (Directory.Exists(moduleDir))
            {
                Directory.Delete(moduleDir, true);
            }
        }

        public void LoadAsset(object bundle, string assetPath, Action<object> onComplete, Action<string> onError)
        {
            AssetBundleRequest request = null;
            try
            {
                AssetBundle assetBundle = (AssetBundle)bundle;
                request = assetBundle.LoadAssetAsync(assetPath);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
                return;
            }
            
            request.completed += (asyncOperation) =>
            {
                onComplete?.Invoke(request.asset);
            };
        }

        public void LoadAsset(object bundle, string assetPath, Type assetType, Action<object> onComplete, Action<string> onError)
        {
            AssetBundleRequest request = null;
            try
            {
                AssetBundle assetBundle = (AssetBundle)bundle;
                request = assetBundle.LoadAssetAsync(assetPath, assetType);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
                return;
            }
            
            request.completed += (asyncOperation) =>
            {
                onComplete?.Invoke(request.asset);
            };
        }

        public void LoadAssetBundle(string bundlePath, Action<object> onComplete, Action<string> onError)
        {
            AssetBundleCreateRequest request = null;
            try
            {
                request = AssetBundle.LoadFromFileAsync(bundlePath);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
                return;
            }

            request.completed += (asyncOperation) =>
            {
                onComplete?.Invoke(request.assetBundle);
            };
        }

        public void InstantiateAsset(object asset, Action<object> onComplete, Action<string> onError)
        {
            object target = null;
            try
            {
                target = UnityObject.Instantiate((UnityObject)asset);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
                return;
            }
            
            onComplete?.Invoke(target);
        }

        public void UnloadAssetBundle(object bundle)
        {
            (bundle as AssetBundle).Unload(true);
        }

        public void UnloadAsset(object asset)
        {
            if (asset is GameObject)
            {
                HQDebugger.LogWarning($"{(asset as GameObject).name} unload failed. You can only destroy individual assets.");
                return;
            }
            Resources.UnloadAsset((UnityObject)asset);
        }

        public void UnloadInstantiatedObject(object @object)
        {
            UnityObject.Destroy((UnityObject)@object);
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
