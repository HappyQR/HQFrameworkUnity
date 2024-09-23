using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public static partial class AssetUtility
    {
        public static void BuildAllAssetModules()
        {
            List<AssetModuleConfig> moduleConfigList = AssetModuleConfig.GetConfigList();
            BuildAssetModules(moduleConfigList);
        }

        public static void BuildAssetModules(List<AssetModuleConfig> moduleConfigList)
        {
            Type preprocessorType = Utility.Assembly.GetType(AssetBuildConfig.Default.preprocessorName);
            Type compilerType = Utility.Assembly.GetType(AssetBuildConfig.Default.compilerName);
            Type postprocessorType = Utility.Assembly.GetType(AssetBuildConfig.Default.postprocessorName);

            IAssetBuildPreprocessor preprocessor = Activator.CreateInstance(preprocessorType) as IAssetBuildPreprocessor;
            IAssetBuildCompiler compiler = Activator.CreateInstance(compilerType) as IAssetBuildCompiler;
            IAssetBuildPostprocessor postprocessor = Activator.CreateInstance(postprocessorType) as IAssetBuildPostprocessor;

            AssetBundleBuild[] builds = preprocessor.PreprocessModules(moduleConfigList);
            AssetBundleManifest buildManifest = compiler.CompileAssets(builds, AssetBuildConfig.Default);
            AssetModuleBuildInfo[] buildResults = postprocessor.PostprocessModules(moduleConfigList, buildManifest);
        }
    }
}
