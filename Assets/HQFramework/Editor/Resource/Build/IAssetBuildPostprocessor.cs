using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQFramework.Editor
{
    public interface IAssetBuildPostprocessor
    {
        void PostProcessAssetBuild(AssetModuleConfig moduleConfig, string[] bundles);
    }
}
