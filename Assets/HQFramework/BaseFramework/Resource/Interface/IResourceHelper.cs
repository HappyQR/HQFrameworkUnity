using System;

namespace HQFramework.Resource
{
    public interface IResourceHelper
    {
        /// <summary>
        /// app launch hotfix id
        /// </summary>
        /// <value></value>
        int LauncherHotfixID
        {
            get;
            set;
        }
        
        HQHotfixMode HotfixMode
        {
            get;
            set;
        }

        string AssetsPersistentDir
        {
            get;
            set;
        }

        string AssetsBuiltinDir
        {
            get;
            set;
        }

        string HotfixManifestUrl
        {
            get;
            set;
        }

        void DecompressBuiltinAssets(Action callback);

        void LoadAssetManifest(Action<ManifestLoadCompleteEventArgs> callback);

        void OverrideLocalManifest(HQAssetManifest localManifest);

        string GetBundleRelatedPath(HQAssetBundleConfig bundleInfo);

        void DeleteAssetModule(HQAssetModuleConfig module);

        void LoadAsset(object bundle, string assetPath, Action<object> onComplete, Action<string> onError);

        void LoadAsset(object bundle, string assetPath, Type assetType, Action<object> onComplete, Action<string> onError);

        void LoadAssetBundle(string bundlePath, Action<object> onComplete, Action<string> onError);
    }
}
