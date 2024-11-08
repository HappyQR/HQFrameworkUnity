using System;
using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace HQFramework.Runtime
{
    internal class DefaultResourceHelper : IResourceHelper
    {
        private static readonly string manifestFileName = "AssetModuleManifest.json";

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

        private string LocalManifestFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(localManifestFilePath))
                {
                    localManifestFilePath = Path.Combine(AssetsPersistentDir, manifestFileName);
                }
                return localManifestFilePath;
            }
        }

        public HQAssetManifest LoadAssetManifest()
        {
            string localManifestJsonStr = File.ReadAllText(LocalManifestFilePath);
            HQAssetManifest localManifest = SerializeManager.JsonToObject<HQAssetManifest>(localManifestJsonStr);
            return localManifest;
        }

        public void OverrideLocalManifest(HQAssetManifest localManifest)
        {
            string manifestJson = SerializeManager.ObjectToJson(localManifest);
            File.WriteAllText(LocalManifestFilePath, manifestJson);
        }

        public string GetBundleFilePath(HQAssetBundleConfig bundleInfo)
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
    }
}
