using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

namespace HQFramework.Editor
{
    public class AssetBuildOptionManager
    {
        private static readonly string bundleBuildOptionPrefsKey = "bundleOption";
        private static readonly string buildOptionDir = "Assets/Config/EditorConfig/Build/";
        public static AssetBuildOption GetDefaultOption()
        {
            string optionPath = EditorPrefs.GetString(bundleBuildOptionPrefsKey);
            if (string.IsNullOrEmpty(optionPath))
            {
                return null;
            }
            AssetBuildOption option = AssetDatabase.LoadAssetAtPath<AssetBuildOption>(optionPath);
            return option;
        }

        public static void SetDefaultOption(AssetBuildOption defaultOption)
        {
            string optionPath = AssetDatabase.GetAssetPath(defaultOption);
            EditorPrefs.SetString(bundleBuildOptionPrefsKey, optionPath);
        }

        public static AssetBuildOption CreateNewOption(string tag)
        {
            if (!AssetDatabase.IsValidFolder(buildOptionDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(buildOptionDir));
                AssetDatabase.Refresh();
            }
            string optionPath = Path.Combine(buildOptionDir, $"{tag}BuildOption.asset");
            AssetBuildOption option = ScriptableObject.CreateInstance<AssetBuildOption>();
            option.tag = tag;
            option.compressOption = CompressOption.LZ4;
            option.platform = (BuildTargetPlatform)EditorUserBuildSettings.activeBuildTarget;
            AssetDatabase.CreateAsset(option, optionPath);
            AssetDatabase.Refresh();
            option = AssetDatabase.LoadAssetAtPath<AssetBuildOption>(optionPath);

            return option;
        }

        public static List<AssetBuildOption> GetOptionList()
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
