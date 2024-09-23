using System.Collections.Generic;

namespace HQFramework.Editor
{
    public interface IAssetBuildPreprocessor
    {
        AssetPreprocessResult PreprocessModules(List<AssetModuleConfig> moduleConfigList);
    }
}
