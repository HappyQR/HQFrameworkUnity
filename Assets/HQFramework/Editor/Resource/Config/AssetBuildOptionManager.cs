using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetBuildOptionManager
    {
        private static readonly string buildOptionPrefsKey = "asset_build_option";
        private static readonly string buildOptionDir = "Assets/Configuration/Editor/Asset/Build/";

        public static AssetBuildOption GetDefaultConfig()
        {
            string path = EditorPrefs.GetString(buildOptionPrefsKey);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            AssetBuildOption option = AssetDatabase.LoadAssetAtPath<AssetBuildOption>(path);
            return option;
        }

        public static void SetDefaultConfig(AssetBuildOption defaultOption)
        {
            string optionPath = AssetDatabase.GetAssetPath(defaultOption);
            EditorPrefs.SetString(buildOptionPrefsKey, optionPath);
        }

        public static AssetBuildOption CreateNewConfig(string tag)
        {
            if (!AssetDatabase.IsValidFolder(buildOptionDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(buildOptionDir));
                AssetDatabase.Refresh();
            }
            string optionPath = Path.Combine(buildOptionDir, $"AssetBuildOption_{tag}.asset");
            AssetBuildOption option = ScriptableObject.CreateInstance<AssetBuildOption>();
            option.optionTag = tag;
            option.compressOption = CompressOption.LZ4;
            option.platform = (BuildTargetPlatform)EditorUserBuildSettings.activeBuildTarget;
            AssetDatabase.CreateAsset(option, optionPath);
            AssetDatabase.Refresh();
            option = AssetDatabase.LoadAssetAtPath<AssetBuildOption>(optionPath);

            return option;
        }

        public static List<AssetBuildOption> GetConfigList()
        {
            List<AssetBuildOption> options = new List<AssetBuildOption>();
            if (!AssetDatabase.IsValidFolder(buildOptionDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(buildOptionDir));
                AssetDatabase.Refresh();
            }
            string[] configs = AssetDatabase.FindAssets("", new[] { buildOptionDir });
            for (int i = 0; i < configs.Length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(configs[i]);
                try
                {
                    options.Add(AssetDatabase.LoadAssetAtPath<AssetBuildOption>(filePath));
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Debug.LogError("Don't put other object under assets build option directory!");
                }
            }
            return options;
        }
    }
}
