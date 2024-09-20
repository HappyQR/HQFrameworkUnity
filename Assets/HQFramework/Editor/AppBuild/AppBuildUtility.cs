using System.Collections.Generic;
using System.IO;
using HQFramework.Version;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AppBuildUtility
    {
        private static readonly string versionFilePath = "Assets/Resources/VersionInfo.json";

        public static void StartBuild(AppBuildConfig config)
        {
            GenerateVersionInfo(config);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenesToBuild();
            BuildOptions options = BuildOptions.None;
            if (config.devBuild)
            {
                options = BuildOptions.Development | BuildOptions.AllowDebugging;
            }
            buildPlayerOptions.options = options;
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
            string originFileName = Path.GetFileNameWithoutExtension(config.buildBundleName);
            string fileExtension = Path.GetExtension(config.buildBundleName);
            string fileName = $"{originFileName}_{config.productVersion.Replace('.', '_')}_{config.internalVersionCode}_{config.versionTag}{fileExtension}";
            buildPlayerOptions.locationPathName = Path.Combine(Application.dataPath, config.bundleOutputDir, fileName);
            
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

            if (report.summary.result == BuildResult.Succeeded)
            {
                if (config.autoIncreaseBuildVersion)
                {
                    config.nextVersionCode++;
                }
                Debug.Log("Build App Successfully");
            }
            else
            {
                Debug.LogError("Build Failed.");
            }
        }

        public static void GenerateVersionInfo(AppBuildConfig config)
        {
            VersionInfo info = new VersionInfo();
            info.productName = config.productName;
            info.companyName = config.companyName;
            info.runtimePlatform = config.runtimePlatform;
            info.productVersion = config.productVersion;
            info.versionTag = config.versionTag;
            info.internalVersionCode = config.nextVersionCode;
            config.internalVersionCode = config.nextVersionCode;
            info.minimalSupportedVersionCode = config.minimalSupportedVersionCode;
            info.releaseNote = config.releaseNote;

            string jsonStr = JsonUtilityEditor.ToJson(info);
            File.WriteAllText(versionFilePath, jsonStr);

            AssetDatabase.Refresh();
        }

        private static string[] GetScenesToBuild()
        {
            List<string> scenes = new List<string>();

            foreach (var item in EditorBuildSettings.scenes)
            {
                scenes.Add(item.path);
            }

            return scenes.ToArray();
        }
    }
}
