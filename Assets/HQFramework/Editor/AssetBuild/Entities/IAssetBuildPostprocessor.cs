namespace HQFramework.Editor
{
    public interface IAssetBuildPostprocessor
    {
        AssetPostprocessData PostprocessAssetModules(AssetCompileData compileData);
    }
}
