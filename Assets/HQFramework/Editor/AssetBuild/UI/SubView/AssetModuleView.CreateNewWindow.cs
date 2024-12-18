using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetModuleView
    {
        private class CreateNewWindow : EditorWindow
        {
            private string moduleName;
            private string devNotes;
            private bool isBuiltin;
            private UnityEngine.Object rootFolder;

            private AssetModuleView parentView;

            public static void ShowWindow(AssetModuleView parent)
            {
                var window = GetWindow<CreateNewWindow>();
                window.parentView = parent;
                window.minSize = window.maxSize = new Vector2(450, 340);
                window.titleContent = new GUIContent("Create New Module");
                window.Show();
            }

            private void OnGUI()
            {
                GUILayout.BeginArea(new Rect(10, 10, position.width - 20, position.height - 20));

                GUIStyle headerStyle = "AM HeaderStyle";

                GUILayout.Label("Module Name:", headerStyle);
                GUILayout.Space(5);
                moduleName = GUILayout.TextField(moduleName);
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Is Built-in:", headerStyle);
                GUILayout.Space(5);
                isBuiltin = GUILayout.Toggle(isBuiltin, "");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.Label("Module Assets Root Folder:", headerStyle);
                GUILayout.Space(5);
                rootFolder = EditorGUILayout.ObjectField(GUIContent.none, rootFolder, typeof(DefaultAsset), false);
                GUILayout.Space(15);

                GUILayout.Label("Module Dev Notes:", headerStyle);
                GUILayout.Space(5);
                devNotes = EditorGUILayout.TextArea(devNotes, GUILayout.Height(120));
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Create New Module", GUILayout.Height(35)))
                {
                    if (string.IsNullOrEmpty(moduleName))
                    {
                        Debug.LogError("Module Name is empty!");
                    }
                    else
                    {
                        AssetModuleConfigAgent config = HQAssetBuildLauncher.CreateModuleConfig(moduleName, isBuiltin, rootFolder, devNotes);
                        parentView.moduleList = HQAssetBuildLauncher.GetModuleConfigs();
                        Close();
                    }
                }

                GUILayout.EndArea();
            }
        }
    }
}
