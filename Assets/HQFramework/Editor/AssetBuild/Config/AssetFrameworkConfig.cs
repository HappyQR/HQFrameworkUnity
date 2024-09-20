using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetFrameworkConfig : ScriptableObject
    {
        private static readonly string frameworkConfigPath = "Assets/Configuration/Editor/Asset/AssetFrameworkConfig.asset";
        private static AssetFrameworkConfig instance;

        public static AssetFrameworkConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<AssetFrameworkConfig>(frameworkConfigPath);
                    if (instance == null)
                    {
                        AssetFrameworkConfig config = ScriptableObject.CreateInstance<AssetFrameworkConfig>();
                        AssetDatabase.CreateAsset(config, frameworkConfigPath);
                        instance = AssetDatabase.LoadAssetAtPath<AssetFrameworkConfig>(frameworkConfigPath);
                    }
                }
                return instance;
            }
        }

        public AssetBuildConfig defaultBuildConfig;

        public AssetRuntimeConfig defaultRuntimeConfig;

        public void Save()
        {
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);
        }
    }
}
