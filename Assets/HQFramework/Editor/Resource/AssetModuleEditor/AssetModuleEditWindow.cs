using UnityEngine;
using UnityEditor;
using HQFramework.Editor;

namespace HQFramework.Editor
{
    public class AssetModuleEditWindow : EditorWindow
    {
        private AssetModuleConfig config;
        private bool createNewConfig;
        private bool saveNewConfig;

        private static string tempDir = "Assets/Config/EditorConfig/AssetModule/";

        public void ShowWindow(AssetModuleConfig target)
        {
            config = target;
            if (config == null)
            {
                int id = AssetModuleManager.GetNewModuleID();
                AssetModuleConfig temp = ScriptableObject.CreateInstance<AssetModuleConfig>();
                temp.id = id;
                temp.currentPatchVersion = 1;
                temp.minimalSupportedPatchVersion = 1;
                string tempPath = tempDir + "temp.asset";
                AssetDatabase.CreateAsset(temp, tempPath);
                config = AssetDatabase.LoadAssetAtPath<AssetModuleConfig>(tempPath);
                createNewConfig = true;
            }
            var window = GetWindow<AssetModuleEditWindow>();
            window.minSize = window.maxSize = new Vector2(480, 360);
            window.titleContent = new GUIContent("Create New Asset Module");
            window.Show();
        }

        private void OnDisable()
        {
            if (createNewConfig && !saveNewConfig)
            {
                AssetDatabase.DeleteAsset(tempDir + "temp.asset");
            }
            else
            {
                if (config != null)
                {
                    EditorUtility.SetDirty(config);
                    AssetDatabase.SaveAssetIfDirty(config);
                }
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
                if (GUILayout.Button("Create New Module"))
                {
                    saveNewConfig = true;
                    // Create New Module
                    if (AssetModuleManager.CreateNewAssetModule(config))
                    {
                        GetWindow<AssetBuildWindow>().RefreshModuleList();
                        Close();
                    }
                }
            }

            GUILayout.EndArea();
        }
    }
}