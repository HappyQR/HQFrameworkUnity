using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetModuleConfigAgent : ScriptableObject
    {
        public int id;
        public string moduleName;
        public string createTime;
        public uint buildVersionCode;
        public string devNotes;

        public UnityEngine.Object rootFolder;
        public bool isBuild;
    }
}
