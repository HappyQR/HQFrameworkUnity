using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public sealed class AssetBuildUtility
    {
        private static readonly string assetBuildCacheFolderName = "AssetBuildCache";

        public static void BuildAllModules()
        {

        }

        public static void BuildModules(List<AssetModuleConfig> moduleList)
        {
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultConfig();
            IAssetBuildPreprocessor preprocesser = new DefaultAssetBuildPreprocessor();
            IAssetBuildCompiler compiler = null;
            compiler.AssetBuildCacheDir = Path.Combine(Application.dataPath, buildOption.bundleOutputDir, assetBuildCacheFolderName);

            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            for (int i = 0; i < moduleList.Count; i++)
            {
                builds.AddRange(preprocesser.PreProcessAssetModuleBuild(moduleList[i]));
            }

            AssetBundleManifest buildManifest = compiler.CompileAssets(builds.ToArray(), buildOption);
        }

        public static void ClearBuildHistory()
        {
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultConfig();
            string bundleOutputDir = Path.Combine(Application.dataPath, buildOption.bundleOutputDir);
            string bundleBuiltinDir = Path.Combine(Application.streamingAssetsPath, buildOption.builtinDir);
            if (Directory.Exists(bundleOutputDir))
                Directory.Delete(bundleOutputDir, true);
            if (Directory.Exists(bundleBuiltinDir))
                Directory.Delete(bundleBuiltinDir, true);

            AssetDatabase.Refresh();

            Debug.Log("Clear Builds Done.");
        }
    }
}
