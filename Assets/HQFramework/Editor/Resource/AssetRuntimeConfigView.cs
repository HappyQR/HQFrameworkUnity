using UnityEngine;
using UnityEditor;
using System.IO;
using HQFramework.Runtime.Resource;

namespace HQFramework.Editor
{
    public class AssetRuntimeConfigView : TabContentView
    {
        private AssetRuntimeConfig config;
        private AssetBuildOption buildOption;
        private static readonly string runtimeConfigPath = "Assets/Resources/AssetRuntimeConfig.asset";

        public AssetRuntimeConfigView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override void OnEnable()
        {
            buildOption = AssetBuildUtility.GetDefaultOption();
            config = AssetDatabase.LoadAssetAtPath<AssetRuntimeConfig>(runtimeConfigPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<AssetRuntimeConfig>();
                AssetDatabase.CreateAsset(config, runtimeConfigPath);
            }

            config.enableHotfix = buildOption.enableHotfix;
            config.hotfixUrl = buildOption.bundleUploadUrl;
            config.hotfixManifestUrl = buildOption.manifestUploadUrl;
            if (!string.IsNullOrEmpty(buildOption.builtinDir))
            {
                config.builtinDir = Path.GetRelativePath(Application.streamingAssetsPath, buildOption.builtinDir);
            }
        }

        public override void OnDisable()
        {
            if (config != null)
            {
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssetIfDirty(config);
            }
        }

        public override void OnGUI()
        {
            GUIStyle headerStyle = "AM HeaderStyle";
            GUILayout.BeginArea(new Rect(10, 10, viewRect.width - 20, viewRect.height - 20));

            if (config.enableHotfix)
            {
                GUI.enabled = false;
                GUILayout.Label("AssetBundle Hotfix URL:", headerStyle);
                GUILayout.Space(5);
                EditorGUILayout.TextField(config.hotfixUrl);

                GUILayout.Space(10);

                GUILayout.Label("AssetBundle Manifest Hotfix URL:", headerStyle);
                GUILayout.Space(5);
                EditorGUILayout.TextField(config.hotfixManifestUrl);

                GUILayout.Space(10);
                GUI.enabled = true;

                GUILayout.Label("Max Hotfix Download Thread Count:", headerStyle);
                GUILayout.Space(5);
                config.maxDownloadThreadCount = EditorGUILayout.IntField(config.maxDownloadThreadCount);
                GUILayout.Space(10);
            }
            else
            {
                GUILayout.Label("Hotfix Disable", headerStyle);
            }

            GUI.enabled = false;
            GUILayout.Label("Asset Built-in Dir( Relative to Application.streamingAssetsPath ):", headerStyle);
            GUILayout.Space(5);
            EditorGUILayout.TextField(config.builtinDir);

            GUILayout.Space(10);
            GUI.enabled = true;

            GUILayout.Label("Asset Persistent Dir( Relative to Application.persistentDataPath ):", headerStyle);
            GUILayout.Space(5);
            config.assetPersistentDir = EditorGUILayout.TextField(config.assetPersistentDir);

            GUILayout.Space(10);

            GUILayout.EndArea();
        }
    }
}
