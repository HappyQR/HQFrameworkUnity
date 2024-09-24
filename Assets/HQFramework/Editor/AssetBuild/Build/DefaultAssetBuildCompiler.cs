using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetBuildCompiler : IAssetBuildCompiler
    {
        public static readonly string cacheFolderName = "AssetBuildCache";
        public static readonly string libraryFolderName = "Library";

        private string buildCacheDir;
        private string libraryDir;

        public AssetCompileResult CompileAssets(AssetPreprocessResult preprocessResult, AssetBuildConfig buildConfig)
        {
            buildCacheDir = Path.Combine(Application.dataPath, buildConfig.assetOutputDir, cacheFolderName);
            libraryDir = Path.Combine(Application.dataPath, buildConfig.assetOutputDir, libraryFolderName);
            if (!Directory.Exists(buildCacheDir))
            {
                Directory.CreateDirectory(buildCacheDir);
            }
            if (!Directory.Exists(libraryDir))
            {
                Directory.CreateDirectory(libraryDir);
            }
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            foreach (var assetBundleBuildInfoList in preprocessResult.moduleBundleBuildsDic.Values)
            {
                for (int i = 0; i < assetBundleBuildInfoList.Count; i++)
                {
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = assetBundleBuildInfoList[i].bundleName;
                    build.assetNames = assetBundleBuildInfoList[i].bundleAssets;
                    builds.Add(build);
                }
            }
            AssetBundleManifest buildManifest = BuildPipeline.BuildAssetBundles(buildCacheDir, builds.ToArray(), (BuildAssetBundleOptions)buildConfig.compressOption, (BuildTarget)buildConfig.platform);

            AssetCompileResult compileResult = new AssetCompileResult();
            foreach (var moduleConfig in preprocessResult.moduleBundleBuildsDic.Keys)
            {
                string[] bundles = GetModuleBundles(moduleConfig, buildManifest);
                AssetBundleBuildResult[] bundleArr = new AssetBundleBuildResult[bundles.Length];
                for (int i = 0; i < bundles.Length; i++)
                {
                    AssetBundleBuildResult bundleBuildResult = new AssetBundleBuildResult();
                    bundleBuildResult.bundleName = bundles[i];
                    string bundleOriginFilePath = Path.Combine(buildCacheDir, bundles[i]);
                    string md5 = Utility.Hash.ComputeHash(bundleOriginFilePath);
                    string bundleDestFilePath = Path.Combine(libraryDir, md5);
                    int fileSize = FileUtilityEditor.GetFileSize(bundleOriginFilePath);
                    File.Copy(bundleOriginFilePath, bundleDestFilePath, true);
                    bundleBuildResult.filePath = bundleDestFilePath;
                    bundleBuildResult.md5 = md5;
                    bundleBuildResult.size = fileSize;
                    bundleBuildResult.dependencies = buildManifest.GetAllDependencies(bundles[i]);
                    bundleArr[i] = bundleBuildResult;
                }
                compileResult.moduleBundleDic.Add(moduleConfig, bundleArr);
            }
            return compileResult;
        }

        private string[] GetModuleBundles(AssetModuleConfig module, AssetBundleManifest buildManifest)
        {
            string[] allBundles = buildManifest.GetAllAssetBundles();
            List<string> bundles = new List<string>();
            string modulePrefix = module.moduleName.ToLower();
            for (int i = 0; i < allBundles.Length; i++)
            {
                if (allBundles[i].StartsWith(modulePrefix))
                {
                    bundles.Add(allBundles[i]);
                }
            }
            return bundles.ToArray();
        }
    }
}
