using System.Collections;
using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetBuildUtility
    {
        public static readonly string assetManifestFileName = "AssetModuleManifest.json";
        public static readonly string builtinManifestFileName = "BuiltinModuleManifest.json";

        public static void GenerateAssetModuleManifest()
        {
            
        }

        public static AssetModuleManifest GetCurrentManifest()
        {
            AssetBuildOption option = AssetBuildOptionManager.GetDefaultConfig();
            string manifestPath = Path.Combine(Application.dataPath, option.bundleOutputDir, assetManifestFileName);
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
            string manifestPath = Path.Combine(Application.streamingAssetsPath, option.builtinDir, builtinManifestFileName);
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
