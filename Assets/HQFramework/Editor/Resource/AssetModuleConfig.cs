using System;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace HQFramework.Editor
{
    [CreateAssetMenu(fileName = "AssetModuleConfig", menuName = "HQFramework/AssetModuleConfig", order = 0)]
    public class AssetModuleConfig : ScriptableObject
    {
        public int id;
        public string moduleName;
        public UnityObject rootFolder;
        public bool isBuiltin;
        public int currentPatchVersion;
        public int minimalSupportedPatchVersion;
        public string description;
        public string releaseNote;
        public bool isBuild;
        public bool autoIncreasePatchVersion;
        public long createTimeTicks;
        public DateTime createTime;
        
        // public DateTime lastModifyTime;
        // public DateTime lastPatchTime;
    }
}