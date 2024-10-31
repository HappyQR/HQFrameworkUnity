using System;
using System.Threading.Tasks;

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
        
        AssetHotfixMode HotfixMode
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

        AssetModuleManifest LoadAssetManifest();

        void OverrideLocalManifest(AssetModuleManifest localManifest);

        string GetBundleFilePath(AssetBundleInfo bundleInfo);

        void DeleteAssetModule(AssetModuleInfo module);

        void LoadAsset(string bundleName, string assetPath, Type assetType, Action<object> callback);
    }
}
