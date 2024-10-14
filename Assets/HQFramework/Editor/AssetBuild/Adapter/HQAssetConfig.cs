using System.Collections.Generic;
using UnityEngine;

namespace HQFramework.Editor
{
    public class HQAssetConfig : ScriptableObject
    {
        public List<AssetBuildConfig> buildConfigList = new List<AssetBuildConfig>();
        public List<AssetModuleConfigAgent> moduleConfigList = new List<AssetModuleConfigAgent>();
    }
}
