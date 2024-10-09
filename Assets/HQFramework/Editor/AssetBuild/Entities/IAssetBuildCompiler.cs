namespace HQFramework.Editor
{
    public interface IAssetBuildCompiler
    {
        AssetCompileData CompileAssetModules(AssetPreprocessData preprocessData, string outputDir, BuildTargetPlatform platform, CompressOption compressOption);
    }
}
