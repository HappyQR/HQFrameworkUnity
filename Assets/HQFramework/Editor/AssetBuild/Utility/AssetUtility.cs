using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public static partial class AssetUtility
    {
        // private static string 

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

            AssetPreprocessResult preprocessResult = preprocessor.PreprocessModules(moduleConfigList);
            AssetCompileResult compileResult = compiler.CompileAssets(preprocessResult, AssetBuildConfig.Default);
            AssetModuleBuildResult[] buildResults = postprocessor.PostprocessModules(compileResult);

            SaveAssetBuilds(buildResults);
        }

        private static void SaveAssetBuilds(AssetModuleBuildResult[] buildResults)
        {
            AssetModuleBuildHistoryData historyData = LoadBuildHistoryData();
            for (int i = 0; i < buildResults.Length; i++)
            {
                AssetModuleBuildResult moduleBuild = buildResults[i];
                if (historyData.moduleBuildData.ContainsKey(moduleBuild.moduleID))
                {
                    historyData.moduleBuildData[moduleBuild.moduleID].Add(moduleBuild);
                }
                else
                {
                    historyData.moduleBuildData.Add(moduleBuild.moduleID, new List<AssetModuleBuildResult>{ moduleBuild });
                }
            }

            // save to local
        }

        public static AssetModuleBuildHistoryData LoadBuildHistoryData()
        {
            return null;
        }
    }
}
