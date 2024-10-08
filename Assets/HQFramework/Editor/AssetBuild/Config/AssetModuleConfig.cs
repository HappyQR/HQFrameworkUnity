using System;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace HQFramework.Editor
{
    public class AssetModuleConfig : ScriptableObject
    {
        public int id;
        public string moduleName;
        public UnityObject rootFolder;
        public uint buildVersionCode;
        public string devNotes;
        public bool isBuild;
        public string createTime;
    }
}
