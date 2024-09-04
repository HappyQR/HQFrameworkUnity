using System;
using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetRuntimeConfigManager
    {
        private static readonly string runtimeConfigPrefsKey = "asset_runtime_config";
        private static readonly string runtimeConfigDir = "Assets/Configuration/Editor/Asset/Runtime/";
        private static readonly string runtimeSettingSavePath = "Assets/Resources/ResourceConfig.json";

        public static AssetRuntimeConfig GetDefaultConfig()
        {
            string path = EditorPrefs.GetString(runtimeConfigPrefsKey);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            AssetRuntimeConfig config = AssetDatabase.LoadAssetAtPath<AssetRuntimeConfig>(path);
            return config;
        }

        public static void SetDefaultConfig(AssetRuntimeConfig defaultConfig)
        {
            string configPath = AssetDatabase.GetAssetPath(defaultConfig);
            EditorPrefs.SetString(runtimeConfigPrefsKey, configPath);
        }

        public static AssetRuntimeConfig CreateNewConfig(string tag)
        {
            if (!AssetDatabase.IsValidFolder(runtimeConfigDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(runtimeConfigDir));
                AssetDatabase.Refresh();
            }
            string configPath = Path.Combine(runtimeConfigDir, $"AssetRuntimeConfig_{tag}.asset");
            AssetRuntimeConfig config = ScriptableObject.CreateInstance<AssetRuntimeConfig>();
            config.tag = tag;
            AssetDatabase.CreateAsset(config, configPath);
            AssetDatabase.Refresh();
            config = AssetDatabase.LoadAssetAtPath<AssetRuntimeConfig>(configPath);

            return config;
        }

        public static List<AssetRuntimeConfig> GetConfigList()
        {
            List<AssetRuntimeConfig> configs = new List<AssetRuntimeConfig>();
            if (!AssetDatabase.IsValidFolder(runtimeConfigDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(runtimeConfigDir));
                AssetDatabase.Refresh();
            }
            string[] results = AssetDatabase.FindAssets("", new[] { runtimeConfigDir });
            for (int i = 0; i < results.Length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(results[i]);
                try
                {
                    configs.Add(AssetDatabase.LoadAssetAtPath<AssetRuntimeConfig>(filePath));
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Debug.LogError("Don't put other object under assets build config directory!");
                }
            }
            return configs;
        }

        public static void GenerateRuntimeSettings(AssetRuntimeConfig config)
        {
            ResourceConfig runtimeConfig = new ResourceConfig();
            runtimeConfig.hotfixMode = config.hotfixMode;
            runtimeConfig.assetBuiltinDir = config.assetBuiltinDir;
            runtimeConfig.assetPersistentDir = config.assetPersistentDir;
            runtimeConfig.hotfixManifestUrl = config.hotfixManifestUrl;
            runtimeConfig.hotfixUrl = config.hotfixUrl;

            string configJson = JsonUtilityEditor.ToJson(runtimeConfig);
            File.WriteAllText(runtimeSettingSavePath, configJson);
            AssetDatabase.Refresh();
        }
    }
}
