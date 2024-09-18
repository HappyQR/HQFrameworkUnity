using UnityEditor;

namespace HQFramework.Editor
{
    public interface IAssetBuildPreprocesser
    {
        AssetBundleBuild[] PreProcessAssetModuleBuild(AssetModuleConfig module);
    }
}
