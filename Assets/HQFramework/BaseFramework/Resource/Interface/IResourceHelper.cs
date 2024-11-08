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

        HQAssetManifest LoadAssetManifest();

        void OverrideLocalManifest(HQAssetManifest localManifest);

        string GetBundleFilePath(HQAssetBundleConfig bundleInfo);

        void DeleteAssetModule(HQAssetModuleConfig module);

        void LoadAsset(object bundle, string assetPath, Type assetType, Action<object> callback);

        void LoadAssetBundle(string bundlePath, Action<object> callback);

        bool IsIndividualAsset(object asset);

        object InstantiateAsset(object asset);

        void UnloadAsset(object asset);

        void UnloadBundle(object bundle);
    }
}
