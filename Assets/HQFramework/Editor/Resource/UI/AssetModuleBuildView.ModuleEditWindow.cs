using System;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetModuleBuildView
    {
        private class ModuleEditWindow : EditorWindow
        {
            private AssetModuleConfig config;
            private Action<AssetModuleConfig> createCallback;
            private bool createNewConfig;
            private bool saveNewConfig;

            private static readonly string tempDir = "Assets/Configuration/Editor/Asset/AssetModule/";

            public void ShowWindow(AssetModuleConfig target, Action<AssetModuleConfig> createCallback)
            {
                config = target;
                this.createCallback = createCallback;
                if (config == null)
                {
                    int id = AssetModuleConfigManager.GetNewModuleID();
                    AssetModuleConfig temp = ScriptableObject.CreateInstance<AssetModuleConfig>();
                    temp.id = id;
                    temp.currentPatchVersion = 1;
                    temp.minimalSupportedPatchVersion = 1;
                    string tempPath = tempDir + "temp.asset";
                    AssetDatabase.CreateAsset(temp, tempPath);
                    config = AssetDatabase.LoadAssetAtPath<AssetModuleConfig>(tempPath);
                    createNewConfig = true;
                }
                var window = GetWindow<ModuleEditWindow>();
                window.minSize = window.maxSize = new Vector2(480, 380);
                window.titleContent = new GUIContent("Asset Module Edit");
                window.Show();
            }

            private void OnDisable()
            {
                if (createNewConfig && !saveNewConfig)
                {
                    AssetDatabase.DeleteAsset(tempDir + "temp.asset");
                }
            }

            private void OnGUI()
            {
                GUILayout.BeginArea(new Rect(10, 10, position.width - 20, position.height - 20));

                GUIStyle headerStyle = "AM HeaderStyle";
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Module ID : {config.id}", headerStyle);
                if (!createNewConfig)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label($"Create Time : {config.createTime:yyyy-MM-dd HH:mm:ss}");
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.Label("Module Name:", headerStyle);
                GUILayout.Space(5);
                config.moduleName = GUILayout.TextField(config.moduleName);
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Is Built-in:", headerStyle);
                GUILayout.Space(5);
                config.isBuiltin = GUILayout.Toggle(config.isBuiltin, "");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.Label("Module Assets Root Folder:", headerStyle);
                GUILayout.Space(5);
                config.rootFolder = EditorGUILayout.ObjectField(GUIContent.none, config.rootFolder, typeof(DefaultAsset), false);
                GUILayout.Space(10);

                GUILayout.Label("Module Description:", headerStyle);
                GUILayout.Space(5);
                config.description = EditorGUILayout.TextArea(config.description, GUILayout.Height(120));
                GUILayout.FlexibleSpace();

                if (createNewConfig)
                {
                    if (GUILayout.Button("Create New Module", GUILayout.Height(35)))
                    {
                        saveNewConfig = true;
                        // Create New Module
                        if (AssetModuleConfigManager.CreateNewAssetModule(config))
                        {
                            createCallback?.Invoke(config);
                            Close();
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("Save", GUILayout.Height(35)))
                    {
                        EditorUtility.SetDirty(config);
                        AssetDatabase.SaveAssetIfDirty(config);
                        Close();
                    }
                }

                GUILayout.EndArea();
            }
        }
    }
}
