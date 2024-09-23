namespace HQFramework.Editor
{
    public interface IAssetBuildPostprocessor
    {
        AssetModuleBuildResult[] PostprocessModules(AssetCompileResult compileResult);
    }
}
