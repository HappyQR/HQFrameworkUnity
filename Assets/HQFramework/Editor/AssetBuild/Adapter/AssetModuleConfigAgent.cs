using System;

namespace HQFramework.Editor
{
    [Serializable]
    public class AssetModuleConfigAgent
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