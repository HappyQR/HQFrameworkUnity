using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetConfigManager
    {
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
            return null;
        }

        public static List<AssetBuildConfig> GetBuildConfigs()
        {
            return null;
        }

        public static List<AssetRuntimeConfig> GetRuntimeConfigs()
        {
            return null;
        }

        public static List<AssetArchiveConfig> GetArchiveConfigs()
        {
            return null;
        }

        public static List<AssetPublishConfig> GetPublishConfigs()
        {
            return null;
        }

        public static AssetModuleConfig CreateNewModuleConfig(string moduleName, UnityEngine.Object rootFolder, string devNotes)
        {
            return null;
        }

        public static AssetBuildConfig CreateNewBuildConfig(string tag)
        {
            return null;
        }

        public static AssetRuntimeConfig CreateNewRuntimeConfig(string tag)
        {
            return null;
        }

        public static AssetArchiveConfig CreateNewArchiveConfig(string tag)
        {
            return null;
        }

        public static AssetPublishConfig CreateNewPublishConfig(string tag)
        {
            return null;
        }

        public static bool DeleteModuleConfig(AssetModuleConfig moduleConfig)
        {
            return false;
        }

        public static void Save()
        {
            AssetFrameworkConfig.Instance.Save();
        }
    }
}
