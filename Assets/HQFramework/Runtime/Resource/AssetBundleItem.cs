using System;
using System.IO;
using HQFramework.Resource;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace HQFramework.Runtime
{
    public class AssetBundleItem : BundleBase
    {
        private AssetBundle bundle;

        protected override bool Ready => bundle != null;

        protected override void LoadAsset<T>(string key)
        {
            
        }

        protected override void LoadAsset(string key, Type assetType)
        {
            
        }

        protected override void LoadBundle()
        {
            string bundleFilePath = "";
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundleFilePath);
            request.completed += (opt)=>
            {
                bundle = (opt as AssetBundleCreateRequest).assetBundle;
            };
        }

        protected override void UnloadAsset(object asset)
        {
            int id = asset.GetHashCode();
            if (loadedAssets.ContainsKey(id))
            {
                UnityObject.Destroy(asset as UnityObject);
                loadedAssets.Remove(id);
                referenceCount--;
            }
        }

        protected override void OnRecyle()
        {
            base.OnRecyle();
            bundle.Unload(true);
            bundle = null;
        }
    }
}
