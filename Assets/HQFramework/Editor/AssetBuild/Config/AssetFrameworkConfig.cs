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

        public List<AssetModuleConfig> assetModuleConfigs;
        public List<AssetBuildConfig> assetBuildConfigs;
        public List<AssetArchiveConfig> assetArchiveConfigs;
        public List<AssetPublishConfig> assetPublishConfigs;
        public List<AssetRuntimeConfig> assetRuntimeConfigs;

        public void Save()
        {
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);
        }
    }
}
