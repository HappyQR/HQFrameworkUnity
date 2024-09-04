using System;
using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetBuildUtility
    {
        public static void BuildAllAssetModules(string releaseNote = null)
        {
            List<AssetModuleConfig> modules = AssetModuleConfigManager.GetModuleList();
            BuildAssetModules(modules, releaseNote);
        }

        public static void BuildAssetModules(List<AssetModuleConfig> modules, string releaseNote = null)
        {
            if (modules == null || modules.Count == 0)
            {
                return;
            }
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultConfig();
            AppBuildConfig appBuildConfig = AppBuildConfigManager.GetDefaultConfig();
            string builderTypeName = "HQFramework.Editor." + buildOption.hotfixMode.ToString() + "Build";
            Type builderType = Type.GetType(builderTypeName);
            HotfixBuild builder = Activator.CreateInstance(builderType, buildOption, appBuildConfig) as HotfixBuild;
            builder.BuildAssetMoudles(modules, releaseNote);
        }

        public static void InspectAssetModules()
        {
            List<AssetModuleConfig> modules = AssetModuleConfigManager.GetModuleList();
            bool result = true;
            for (int i = 0; i < modules.Count; i++)
            {
                AssetModuleConfig module = modules[i];
                if (string.IsNullOrEmpty(module.moduleName))
                {
                    Debug.LogError($"There is a module without name : module ID : {module.id}");
                    result = false;
                }

                if (module.rootFolder == null)
                {
                    Debug.LogError($"There is a module without root reference : module ID : {module.id}, module name : {module.moduleName}");
                    result = false;
                }
                else
                {
                    string rootDir = AssetDatabase.GetAssetPath(module.rootFolder);
                    string[] assets = AssetDatabase.FindAssets("", new string[] { rootDir });
                    if (assets == null || assets.Length == 0)
                    {
                        Debug.LogError($"There is a module without assets : module ID : {module.id}, module name : {module.moduleName}");
                        result = false;
                    }
                }
            }
            if (result)
            {
                Debug.Log("Asset Modules Check Done, Clean.");
            }
        }

        public static void ClearBuilds()
        {
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultConfig();
            string bundleOutputDir = Path.Combine(Application.dataPath, buildOption.bundleOutputDir);
            string bundleBuiltinDir = Path.Combine(Application.streamingAssetsPath, buildOption.builtinDir);
            if (Directory.Exists(bundleOutputDir))
                Directory.Delete(bundleOutputDir, true);
            if (Directory.Exists(bundleBuiltinDir))
                Directory.Delete(bundleBuiltinDir, true);

            AssetDatabase.Refresh();

            Debug.Log("Clear All Builds Done.");
        }
    }
}
