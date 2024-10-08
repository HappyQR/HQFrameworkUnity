using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetFrameworkConfig : ScriptableObject
    {
        private static readonly string frameworkConfigDir = "Assets/Configuration/Editor/Asset/";
        private static readonly string frameworkConfigPath = "Assets/Configuration/Editor/Asset/AssetFrameworkConfig.asset";
        private static AssetFrameworkConfig instance;

        public static AssetFrameworkConfig Instance
        {
            get
            {
                if (!AssetDatabase.IsValidFolder(frameworkConfigDir))
                {
                    Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(frameworkConfigDir));
                    AssetDatabase.Refresh();
                }
                if (instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<AssetFrameworkConfig>(frameworkConfigPath);
                    if (instance == null)
                    {
                        AssetFrameworkConfig config = ScriptableObject.CreateInstance<AssetFrameworkConfig>();
                        AssetDatabase.CreateAsset(config, frameworkConfigPath);
                        instance = AssetDatabase.LoadAssetAtPath<AssetFrameworkConfig>(frameworkConfigPath);
                    }
                }
                return instance;
            }
        }

        public AssetBuildConfig defaultBuildConfig;
        public AssetRuntimeConfig defaultRuntimeConfig;
        public AssetArchiveConfig defaultArchiveConfig;
        public AssetPublishConfig defaultPublishConfig;

        public List<AssetModuleConfig> assetModuleConfigs = new List<AssetModuleConfig>();
        public List<AssetBuildConfig> assetBuildConfigs = new List<AssetBuildConfig>();
        public List<AssetArchiveConfig> assetArchiveConfigs = new List<AssetArchiveConfig>();
        public List<AssetPublishConfig> assetPublishConfigs = new List<AssetPublishConfig>();
        public List<AssetRuntimeConfig> assetRuntimeConfigs = new List<AssetRuntimeConfig>();

        public void Save()
        {
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);
        }
    }
}
