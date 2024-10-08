using System;
using System.Collections.Generic;

namespace HQFramework.Editor
{
    public sealed class AssetBuilder
    {
        public static void BuildAllAssetModules()
        {
            List<AssetModuleConfig> moduleConfigList = AssetConfigManager.GetModuleConfigs();
            BuildAssetModules(moduleConfigList);
        }

        public static void BuildAssetModules(List<AssetModuleConfig> moduleConfigList)
        {
            Type preprocessorType = Utility.Assembly.GetType(AssetConfigManager.CurrentBuildConfig.preprocessorName);
            Type compilerType = Utility.Assembly.GetType(AssetConfigManager.CurrentBuildConfig.compilerName);
            Type postprocessorType = Utility.Assembly.GetType(AssetConfigManager.CurrentBuildConfig.postprocessorName);

            IAssetBuildPreprocessor preprocessor = Activator.CreateInstance(preprocessorType) as IAssetBuildPreprocessor;
            IAssetBuildCompiler compiler = Activator.CreateInstance(compilerType) as IAssetBuildCompiler;
            IAssetBuildPostprocessor postprocessor = Activator.CreateInstance(postprocessorType) as IAssetBuildPostprocessor;

            AssetPreprocessResult preprocessResult = preprocessor.PreprocessModules(moduleConfigList);
            AssetCompileResult compileResult = compiler.CompileAssets(preprocessResult, AssetConfigManager.CurrentBuildConfig);
            AssetModuleBuildResult[] buildResults = postprocessor.PostprocessModules(compileResult);

            AssetArchiver.AddModuleBuildeResults(buildResults);
        }
    }
}
