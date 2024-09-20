using System.Collections.Generic;
using UnityEngine;

namespace HQFramework.Editor
{
    public interface IAssetBuildPostprocessor
    {
        void PostprocessModules(List<AssetModuleConfig> moduleConfigList, AssetBundleManifest buildManifest);
    }
}
