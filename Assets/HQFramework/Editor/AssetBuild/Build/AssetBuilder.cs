using System;
using System.Collections.Generic;

namespace HQFramework.Editor
{
    public sealed class AssetBuilder
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

            AssetPreprocessResult preprocessResult = preprocessor.PreprocessModules(moduleConfigList);
            AssetCompileResult compileResult = compiler.CompileAssets(preprocessResult, AssetBuildConfig.Default);
            AssetModuleBuildResult[] buildResults = postprocessor.PostprocessModules(compileResult);

            AssetArchiver.AddModuleBuildeResults(buildResults);
        }
    }
}
