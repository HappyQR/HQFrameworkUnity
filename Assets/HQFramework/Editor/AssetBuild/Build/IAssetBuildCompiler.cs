namespace HQFramework.Editor
{
    public interface IAssetBuildCompiler
    {
        AssetCompileResult CompileAssets(AssetPreprocessResult preprocessResult, AssetBuildConfig buildConfig);
    }
}
