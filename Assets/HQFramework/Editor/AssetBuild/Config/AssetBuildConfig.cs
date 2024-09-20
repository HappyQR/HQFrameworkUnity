using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetBuildConfig : ScriptableObject
    {
        public string optionTag;
        public string preprocessorName;
        public string compilerName;
        public string postprocessorName;
        public string assetOutputDir;
        public BuildTargetPlatform platform;
        public CompressOption compressOption;

        // public bool enableEncryption;
    }

    public partial class AssetBuildConfig
    {
        private static readonly string buildOptionDir = "Assets/Configuration/Editor/Asset/Build/";

        public static AssetBuildConfig Default
        {
            get
            {
                return AssetFrameworkConfig.Instance.defaultBuildConfig;
            }
            set
            {
                AssetFrameworkConfig.Instance.defaultBuildConfig = value;
                AssetFrameworkConfig.Instance.Save();
            }
        }

        public static AssetBuildConfig CreateNewConfig(string tag)
        {
            if (!AssetDatabase.IsValidFolder(buildOptionDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(buildOptionDir));
                AssetDatabase.Refresh();
            }
            string optionPath = Path.Combine(buildOptionDir, $"AssetBuildConfig_{tag}.asset");
            AssetBuildConfig option = ScriptableObject.CreateInstance<AssetBuildConfig>();
            option.optionTag = tag;
            option.compressOption = CompressOption.LZ4;
            option.platform = (BuildTargetPlatform)EditorUserBuildSettings.activeBuildTarget;
            AssetDatabase.CreateAsset(option, optionPath);
            AssetDatabase.Refresh();
            option = AssetDatabase.LoadAssetAtPath<AssetBuildConfig>(optionPath);

            return option;
        }

        public static List<AssetBuildConfig> GetConfigList()
        {
            List<AssetBuildConfig> options = new List<AssetBuildConfig>();
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
                    options.Add(AssetDatabase.LoadAssetAtPath<AssetBuildConfig>(filePath));
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

    public enum BuildTargetPlatform
    {
        Android = UnityEditor.BuildTarget.Android,
        iOS = UnityEditor.BuildTarget.iOS,
        StandaloneOSX = UnityEditor.BuildTarget.StandaloneOSX,
        StandaloneWindows = UnityEditor.BuildTarget.StandaloneWindows,
        StandaloneWindows64 = UnityEditor.BuildTarget.StandaloneWindows64,
        VisionOS = UnityEditor.BuildTarget.VisionOS,
        WebGL = UnityEditor.BuildTarget.WebGL
    }

    public enum CompressOption
    {
        LZ4 = UnityEditor.BuildAssetBundleOptions.ChunkBasedCompression,
        NoCompress = UnityEditor.BuildAssetBundleOptions.UncompressedAssetBundle
    }
}
