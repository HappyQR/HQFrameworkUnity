using System;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetModuleView
    {
        private class ModuleEditWindow : EditorWindow
        {
            private AssetModuleConfig config;
            private Action<AssetModuleConfig> createCallback;
            private bool createNewConfig;

            public static void ShowWindow(AssetModuleConfig target, Action<AssetModuleConfig> createCallback)
            {
                var window = GetWindow<ModuleEditWindow>();
                window.config = target;
                window.createCallback = createCallback;
                if (window.config == null)
                {
                    int id = AssetModuleConfig.GetNewModuleID();
                    window.config = ScriptableObject.CreateInstance<AssetModuleConfig>();
                    window.config.id = id;
                    window.createNewConfig = true;
                }
                window.minSize = window.maxSize = new Vector2(480, 380);
                window.titleContent = new GUIContent("Asset Module Edit");
                window.Show();
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

                GUILayout.Label("Module Assets Root Folder:", headerStyle);
                GUILayout.Space(5);
                config.rootFolder = EditorGUILayout.ObjectField(GUIContent.none, config.rootFolder, typeof(DefaultAsset), false);
                GUILayout.Space(15);

                GUILayout.Label("Module Dev Notes:", headerStyle);
                config.devNotes = EditorGUILayout.TextArea(config.devNotes, GUILayout.Height(150));
                GUILayout.FlexibleSpace();

                if (createNewConfig)
                {
                    if (GUILayout.Button("Create New Module", GUILayout.Height(35)))
                    {
                        if (string.IsNullOrEmpty(config.moduleName))
                        {
                            Debug.LogError("Module Name is empty!");
                        }
                        else
                        {
                            AssetModuleConfig.CreateNewConfig(config);
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
