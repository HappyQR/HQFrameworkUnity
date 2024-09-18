using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetBuildUtility
    {
        public static void BuildAllModules()
        {

        }

        public static void BuildModules(List<AssetModuleConfig> moduleList, string releaseNotes = null)
        {
            
        }

        public static void BuildModulesWithoutBuiltin(List<AssetModuleConfig> moduleList)
        {
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultConfig();
            if (buildOption.hotfixMode != Resource.AssetHotfixMode.SeparateHotfix)
            {
                throw new InvalidOperationException("BuildModulesWithoutBuiltin() is just used in SeparateHotfix mode.");
            }

            
        }

        public static void ClearBuildCache()
        {
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultConfig();
            string bundleOutputDir = Path.Combine(Application.dataPath, buildOption.bundleOutputDir);
            string bundleBuiltinDir = Path.Combine(Application.streamingAssetsPath, buildOption.builtinDir);
            if (Directory.Exists(bundleOutputDir))
                Directory.Delete(bundleOutputDir, true);
            if (Directory.Exists(bundleBuiltinDir))
                Directory.Delete(bundleBuiltinDir, true);

            AssetDatabase.Refresh();

            Debug.Log("Clear Builds Done.");
        }
    }
}
