using UnityEditor;

namespace HQFramework.Editor
{
    public interface IAssetBuildPreprocessor
    {
        AssetBundleBuild[] PreProcessAssetModuleBuild(AssetModuleConfig module);
    }
}
