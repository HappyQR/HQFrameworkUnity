using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AppBuildConfigManager
    {
     private static readonly string appBuildConfigPrefsKey = "app_build_config";
        private static readonly string appBuildConfigDir = "Assets/Configuration/Editor/AppBuild/";

        public static AppBuildConfig GetDefaultConfig()
        {
            string path = EditorPrefs.GetString(appBuildConfigPrefsKey);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            AppBuildConfig config = AssetDatabase.LoadAssetAtPath<AppBuildConfig>(path);
            return config;
        }

        public static void SetDefaultConfig(AppBuildConfig defaultConfig)
        {
            string configPath = AssetDatabase.GetAssetPath(defaultConfig);
            EditorPrefs.SetString(appBuildConfigPrefsKey, configPath);
        }

        public static AppBuildConfig CreateNewConfig(string tag)
        {
            if (!AssetDatabase.IsValidFolder(appBuildConfigDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(appBuildConfigDir));
                AssetDatabase.Refresh();
            }
            string configPath = Path.Combine(appBuildConfigDir, $"AppBuildConfig_{tag}.asset");
            AppBuildConfig config = ScriptableObject.CreateInstance<AppBuildConfig>();
            config.tag = tag;
            AssetDatabase.CreateAsset(config, configPath);
            AssetDatabase.Refresh();
            config = AssetDatabase.LoadAssetAtPath<AppBuildConfig>(configPath);

            return config;
        }

        public static List<AppBuildConfig> GetConfigList()
        {
            List<AppBuildConfig> configs = new List<AppBuildConfig>();
            if (!AssetDatabase.IsValidFolder(appBuildConfigDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(appBuildConfigDir));
                AssetDatabase.Refresh();
            }
            string[] results = AssetDatabase.FindAssets("", new[] { appBuildConfigDir });
            for (int i = 0; i < results.Length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(results[i]);
                try
                {
                    AppBuildConfig config = AssetDatabase.LoadAssetAtPath<AppBuildConfig>(filePath);
                    configs.Add(config);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Debug.LogError("Don't put other object under assets build config directory!");
                }
            }
            return configs;
        }
    }
}
