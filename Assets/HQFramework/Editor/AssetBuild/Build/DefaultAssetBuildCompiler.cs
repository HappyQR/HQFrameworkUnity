using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetBuildCompiler : IAssetBuildCompiler
    {
        private static readonly string cacheFolderName = "AssetBuildCache";

        public AssetBundleManifest CompileAssets(AssetBundleBuild[] builds, AssetBuildConfig buildConfig)
        {
            string buildCacheDir = Path.Combine(Application.dataPath, buildConfig.assetOutputDir, cacheFolderName);
            if (!Directory.Exists(buildCacheDir))
            {
                Directory.CreateDirectory(buildCacheDir);
            }
            return BuildPipeline.BuildAssetBundles(buildCacheDir, builds, (BuildAssetBundleOptions)buildConfig.compressOption, (BuildTarget)buildConfig.platform);
        }
    }
}
