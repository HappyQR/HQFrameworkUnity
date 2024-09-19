using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    public abstract class BundleBase : IReference
    {
        internal protected uint referenceCount;

        protected AssetBundleInfo bundleInfo;
        
        protected Dictionary<int, object> loadedAssets;

        internal protected abstract bool Ready { get; }

        internal protected abstract void LoadBundle();

        internal protected abstract void LoadAsset<T>(string key);

        internal protected abstract void LoadAsset(string key, Type assetType);

        internal protected abstract void UnloadAsset(object asset);

        protected virtual void OnRecyle()
        {
            referenceCount = 0;
            bundleInfo = null;
        }

        void IReference.OnRecyle()
        {
            OnRecyle();
        }
    }
}
