using System;
using System.Collections.Generic;

namespace HQFramework.Editor
{
    public sealed class AssetBuildController
    {
        private static IAssetBuildPreprocessor preprocessor;
        private static IAssetBuildCompiler compiler;
        private static IAssetBuildPostprocessor postprocessor;

        public static void SetPreprocessor(IAssetBuildPreprocessor targetPreprocessor)
        {
            preprocessor = targetPreprocessor;
        }

        public static void SetCompiler(IAssetBuildCompiler targetCompiler)
        {
            compiler = targetCompiler;
        }

        public static void SetPostprocessor(IAssetBuildPostprocessor targetPostprocessor)
        {
            postprocessor = targetPostprocessor;
        }

        public static AssetPostprocessData BuildAssetModules(List<AssetModuleConfig> moduleConfigList, string outputDir, BuildTargetPlatform platform, CompressOption compressOption)
        {
            if (!InitializationCheck())
            {
                throw new InvalidOperationException("You need to initialize controller before build assets");
            }

            try
            {
                AssetPreprocessData preprocessData = preprocessor.PreprocessAssetModules(moduleConfigList);
                AssetCompileData compileData = compiler.CompileAssetModules(preprocessData, outputDir, platform, compressOption);
                AssetPostprocessData postprocessData = postprocessor.PostprocessAssetModules(compileData);

                return postprocessData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool InitializationCheck()
        {
            return preprocessor != null && compiler != null && postprocessor != null;
        }
    }
}
