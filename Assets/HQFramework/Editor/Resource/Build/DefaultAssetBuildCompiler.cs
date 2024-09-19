using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetBuildCompiler : IAssetBuildCompiler
    {
        private string assetBuildCacheDir;

        public string AssetBuildCacheDir 
        { 
            set => assetBuildCacheDir = value; 
        }

        public AssetBundleManifest CompileAssets(AssetBundleBuild[] builds, AssetBuildOption buildOption)
        {
            return BuildPipeline.BuildAssetBundles(assetBuildCacheDir, builds, (BuildAssetBundleOptions)buildOption.compressOption, (BuildTarget)buildOption.platform);
        }
    }
}
