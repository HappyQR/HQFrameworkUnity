using System.Collections.Generic;
using UnityEngine;

namespace HQFramework.Editor
{
    public interface IAssetBuildPostprocessor
    {
        AssetModuleBuildInfo[] PostprocessModules(List<AssetModuleConfig> moduleConfigList, AssetBundleManifest buildManifest);
    }
}
