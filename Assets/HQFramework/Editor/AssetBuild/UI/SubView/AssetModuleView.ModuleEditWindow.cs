using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetModuleView
    {
        private class ModuleEditWindow : EditorWindow
        {
            private AssetModuleConfigAgent config;

            public static void ShowWindow(AssetModuleConfigAgent target)
            {
                var window = GetWindow<ModuleEditWindow>();
                window.config = target;
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
                GUILayout.FlexibleSpace();
                GUILayout.Label($"Create Time : {config.createTime}");
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.Label($"Next Build Code : {config.buildVersionCode}", headerStyle);
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
                config.devNotes = EditorGUILayout.TextArea(config.devNotes, GUILayout.Height(130));
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save", GUILayout.Height(35)))
                {
                    Close();
                }

                GUILayout.EndArea();
            }
        }
    }
}
