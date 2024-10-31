using System;
using System.IO;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Runtime
{
    internal class DefaultResourceHelper : IResourceHelper
    {
        private static readonly string manifestFileName = "AssetModuleManifest.json";

        private string localManifestFilePath;

        public int LauncherHotfixID
        {
            get;
            set;
        }

        public AssetHotfixMode HotfixMode
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

        public AssetModuleManifest LoadAssetManifest()
        {
            string localManifestJsonStr = File.ReadAllText(LocalManifestFilePath);
            AssetModuleManifest localManifest = SerializeManager.JsonToObject<AssetModuleManifest>(localManifestJsonStr);
            return localManifest;
        }

        public void OverrideLocalManifest(AssetModuleManifest localManifest)
        {
            string manifestJson = SerializeManager.ObjectToJson(localManifest);
            File.WriteAllText(LocalManifestFilePath, manifestJson);
        }

        public string GetBundleFilePath(AssetBundleInfo bundleInfo)
        {
            return Path.Combine(AssetsPersistentDir, bundleInfo.moduleID.ToString(), bundleInfo.bundleName);
        }

        public void DeleteAssetModule(AssetModuleInfo module)
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
    }
}
