using UnityEditor;

namespace HQFramework.Editor
{
    public interface IAssetBuildPreprocessor
    {
        AssetBundleBuild[] PreprocessModuleBuild(AssetModuleConfig module);
        bool CheckAllModulesFormat();
    }
}
