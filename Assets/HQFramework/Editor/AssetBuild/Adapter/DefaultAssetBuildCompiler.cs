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

        public AssetCompileData CompileAssetModules(AssetPreprocessData preprocessData, string outputDir, BuildTargetPlatform platform, CompressOption compressOption)
        {
            AssetCompileData compileData = new AssetCompileData();
            buildCacheDir = Path.Combine(outputDir, cacheFolderName);
            libraryDir = Path.Combine(outputDir, libraryFolderName);
            if (!Directory.Exists(buildCacheDir))
            {
                Directory.CreateDirectory(buildCacheDir);
            }
            if (!Directory.Exists(libraryDir))
            {
                Directory.CreateDirectory(libraryDir);
            }
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            foreach (var moduleConfig in preprocessData.moduleConfigList)
            {
                compileData.dataDic.Add(moduleConfig, new List<AssetBundleCompileInfo>());
                for (int i = 0; i < moduleConfig.bundleConfigList.Count; i++)
                {
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = moduleConfig.bundleConfigList[i].bundleName;
                    build.assetNames = moduleConfig.bundleConfigList[i].assetItemList.ToArray();
                    builds.Add(build);
                }
            }
            AssetBundleManifest buildManifest = BuildPipeline.BuildAssetBundles(buildCacheDir, builds.ToArray(), (BuildAssetBundleOptions)compressOption, (BuildTarget)platform);

            foreach (var moduleConfig in compileData.dataDic.Keys)
            {
                string[] bundles = GetModuleBundles(moduleConfig, buildManifest);
                AssetBundleCompileInfo[] bundleArr = new AssetBundleCompileInfo[bundles.Length];
                for (int i = 0; i < bundles.Length; i++)
                {
                    AssetBundleCompileInfo compileInfo = new AssetBundleCompileInfo();
                    compileInfo.bundleName = bundles[i];
                    string bundleOriginFilePath = Path.Combine(buildCacheDir, bundles[i]);
                    string md5 = Utility.Hash.ComputeHash(bundleOriginFilePath);
                    string bundleDestFilePath = Path.Combine(libraryDir, md5);
                    int fileSize = FileUtilityEditor.GetFileSize(bundleOriginFilePath);
                    File.Copy(bundleOriginFilePath, bundleDestFilePath, true);
                    compileInfo.filePath = bundleDestFilePath;
                    compileInfo.md5 = md5;
                    compileInfo.size = fileSize;
                    compileInfo.dependencies = buildManifest.GetAllDependencies(bundles[i]);
                    bundleArr[i] = compileInfo;
                }
                compileData.dataDic[moduleConfig].AddRange(bundleArr);
            }
            return compileData;
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
