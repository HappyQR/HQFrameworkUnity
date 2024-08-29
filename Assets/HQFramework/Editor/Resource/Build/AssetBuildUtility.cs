using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetBuildUtility
    {
        public static readonly string assetManifestFileName = "AssetModuleManifest.json";

        public static void BuildAllAssetModules()
        {
            List<AssetModuleConfig> modules = AssetModuleConfigManager.GetModuleList();
            BuildAssetModules(modules);
        }

        public static void BuildAssetModules(List<AssetModuleConfig> modules)
        {

        }

        public static void GenerateAssetManifest(Dictionary<int, AssetModuleInfo> moduleDic)
        {
            
        }

        public static AssetModuleManifest GetCurrentManifest()
        {
            AssetBuildOption option = AssetBuildOptionManager.GetDefaultConfig();
            AppBuildConfig appBuildConfig = AppBuildConfigManager.GetDefaultConfig();
            string manifestPath = Path.Combine(Application.dataPath, option.bundleOutputDir, appBuildConfig.internalVersionCode.ToString(), assetManifestFileName);
            if (File.Exists(manifestPath))
            {
                string manifestJsonStr = File.ReadAllText(manifestPath);
                AssetModuleManifest manifest = JsonUtilityEditor.ToObject<AssetModuleManifest>(manifestJsonStr);
                return manifest;
            }

            return null;
        }

        public static AssetModuleManifest GetCurrentBuiltinManifest()
        {
            AssetBuildOption option = AssetBuildOptionManager.GetDefaultConfig();
            string manifestPath = Path.Combine(Application.streamingAssetsPath, option.builtinDir, assetManifestFileName);
            if (File.Exists(manifestPath))
            {
                string manifestJsonStr = File.ReadAllText(manifestPath);
                AssetModuleManifest manifest = JsonUtilityEditor.ToObject<AssetModuleManifest>(manifestJsonStr);
                return manifest;
            }

            return null;
        }
    }
}
