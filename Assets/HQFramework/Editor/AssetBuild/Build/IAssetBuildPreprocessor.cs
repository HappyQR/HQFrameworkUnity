using System.Collections.Generic;
using UnityEditor;

namespace HQFramework.Editor
{
    public interface IAssetBuildPreprocessor
    {
        AssetBundleBuild[] PreprocessModules(List<AssetModuleConfig> moduleConfigList);
    }
}
