using System;
using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetConfigManager
    {
        private static readonly string runtimeSettingSavePath = "Assets/Resources/ResourceConfig.json";

        public static AssetBuildConfig CurrentBuildConfig
        {
            get
            {
                if (AssetFrameworkConfig.Instance.defaultBuildConfig == null)
                {
                    AssetFrameworkConfig.Instance.defaultBuildConfig = CreateNewBuildConfig("Default");
                }
                return AssetFrameworkConfig.Instance.defaultBuildConfig;
            }
            set
            {
                AssetFrameworkConfig.Instance.defaultBuildConfig = value;
            }
        }

        public static AssetRuntimeConfig CurrentRuntimeConfig
        {
            get
            {
                if (AssetFrameworkConfig.Instance.defaultRuntimeConfig == null)
                {
                    AssetFrameworkConfig.Instance.defaultRuntimeConfig = CreateNewRuntimeConfig("Default");
                }
                return AssetFrameworkConfig.Instance.defaultRuntimeConfig;
            }
            set
            {
                AssetFrameworkConfig.Instance.defaultRuntimeConfig = value;
            }
        }

        public static AssetArchiveConfig CurrentArchiveConfig
        {
            get
            {
                if (AssetFrameworkConfig.Instance.defaultArchiveConfig == null)
                {
                    AssetFrameworkConfig.Instance.defaultArchiveConfig = CreateNewArchiveConfig("Default");
                }
                return AssetFrameworkConfig.Instance.defaultArchiveConfig;
            }
            set
            {
                AssetFrameworkConfig.Instance.defaultArchiveConfig = value;
            }
        }

        public static AssetPublishConfig CurrentPublishConfig
        {
            get
            {
                if (AssetFrameworkConfig.Instance.defaultPublishConfig == null)
                {
                    AssetFrameworkConfig.Instance.defaultPublishConfig = CreateNewPublishConfig("Default");
                }
                return AssetFrameworkConfig.Instance.defaultPublishConfig;
            }
            set
            {
                AssetFrameworkConfig.Instance.defaultPublishConfig = value;
            }
        }

        public static List<AssetModuleConfig> GetModuleConfigs()
        {
            return AssetFrameworkConfig.Instance.assetModuleConfigs;
        }

        public static List<AssetBuildConfig> GetBuildConfigs()
        {
            return AssetFrameworkConfig.Instance.assetBuildConfigs;
        }

        public static List<AssetRuntimeConfig> GetRuntimeConfigs()
        {
            return AssetFrameworkConfig.Instance.assetRuntimeConfigs;
        }

        public static List<AssetArchiveConfig> GetArchiveConfigs()
        {
            return AssetFrameworkConfig.Instance.assetArchiveConfigs;
        }

        public static List<AssetPublishConfig> GetPublishConfigs()
        {
            return AssetFrameworkConfig.Instance.assetPublishConfigs;
        }

        public static AssetModuleConfig CreateNewModuleConfig(string moduleName, UnityEngine.Object rootFolder, string devNotes)
        {
            AssetModuleConfig config = ScriptableObject.CreateInstance<AssetModuleConfig>();
            config.moduleName = moduleName;
            config.rootFolder = rootFolder;
            config.devNotes = devNotes;
            int moduleID = 0;
            int moduleCount = AssetFrameworkConfig.Instance.assetModuleConfigs.Count;
            if (moduleCount > 0)
            {
                moduleID = AssetFrameworkConfig.Instance.assetModuleConfigs[moduleCount - 1].id++;
            }
            config.id = moduleID;
            config.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            AssetFrameworkConfig.Instance.assetModuleConfigs.Add(config);
            Save();
            return config;
        }

        public static AssetBuildConfig CreateNewBuildConfig(string tag)
        {
            AssetBuildConfig config = ScriptableObject.CreateInstance<AssetBuildConfig>();
            config.optionTag = tag;
            config.compressOption = CompressOption.LZ4;
            config.platform = (BuildTargetPlatform)EditorUserBuildSettings.activeBuildTarget;
            AssetFrameworkConfig.Instance.assetBuildConfigs.Add(config);
            Save();
            return config;
        }

        public static AssetRuntimeConfig CreateNewRuntimeConfig(string tag)
        {
            AssetRuntimeConfig config = ScriptableObject.CreateInstance<AssetRuntimeConfig>();
            config.tag = tag;
            AssetFrameworkConfig.Instance.assetRuntimeConfigs.Add(config);
            Save();
            return config;
        }

        public static AssetArchiveConfig CreateNewArchiveConfig(string tag)
        {
            AssetArchiveConfig config = ScriptableObject.CreateInstance<AssetArchiveConfig>();
            config.tag = tag;
            AssetFrameworkConfig.Instance.assetArchiveConfigs.Add(config);
            Save();
            return config;
        }

        public static AssetPublishConfig CreateNewPublishConfig(string tag)
        {
            AssetPublishConfig config = ScriptableObject.CreateInstance<AssetPublishConfig>();
            config.tag = tag;
            AssetFrameworkConfig.Instance.assetPublishConfigs.Add(config);
            Save();
            return config;
        }

        public static bool DeleteModuleConfig(AssetModuleConfig moduleConfig)
        {
            bool result = AssetFrameworkConfig.Instance.assetModuleConfigs.Remove(moduleConfig);
            Save();
            return result;
        }

        public static void GenerateRuntimeSettings(AssetRuntimeConfig config)
        {
            ResourceConfig runtimeConfig = new ResourceConfig();
            runtimeConfig.hotfixMode = config.hotfixMode;
            runtimeConfig.assetBuiltinDir = config.assetBuiltinDir;
            runtimeConfig.assetPersistentDir = config.assetPersistentDir;
            runtimeConfig.hotfixManifestUrl = config.hotfixManifestUrl;

            string configJson = JsonUtilityEditor.ToJson(runtimeConfig);
            File.WriteAllText(runtimeSettingSavePath, configJson);
            AssetDatabase.Refresh();
            Debug.Log("Runtime Assets Config Generated!");
        }

        public static void Save()
        {
            AssetFrameworkConfig.Instance.Save();
        }
    }
}
