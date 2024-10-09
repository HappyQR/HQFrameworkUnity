using System.Collections.Generic;

namespace HQFramework.Editor
{
    public interface IAssetBuildPreprocessor
    {
        AssetPreprocessData PreprocessAssetModules(List<AssetModuleConfig> moduleConfigList);
    }
}
