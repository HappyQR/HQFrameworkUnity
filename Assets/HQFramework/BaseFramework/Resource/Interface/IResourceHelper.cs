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
        }
        
        AssetHotfixMode HotfixMode
        {
            get;
        }

        string AssetsPersistentDir
        {
            get;
        }

        string AssetsBuiltinDir
        {
            get;
        }

        string HotfixManifestUrl
        {
            get;
        }

        Task<AssetModuleManifest> LoadAssetManifestAsync();

        void OverrideLocalManifest(AssetModuleManifest localManifest);

        string GetBundleFilePath(AssetBundleInfo bundleInfo);

        void DeleteAssetModule(AssetModuleInfo module);
    }
}
