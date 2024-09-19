using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetModuleBuildView
    {
        private class ConfirmWindow : EditorWindow
        {
            private List<AssetModuleConfig> modules;
            private AssetBuildOption buildOption;
            private string releaseNote;
            private Vector2 scrollPos;

            public void ShowWindow(List<AssetModuleConfig> modules, AssetBuildOption buildOption)
            {
                this.modules = modules;
                this.buildOption = buildOption;
                ConfirmWindow window = GetWindow<ConfirmWindow>();
                window.titleContent = new GUIContent("Build Assets");
                window.maxSize = window.minSize = new Vector2(480, 420);
                window.Show();
            }

            private void OnGUI()
            {
                GUILayout.BeginArea(new Rect(10, 10, position.width - 20, position.height - 20));
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                scrollPos.x = 0;
                GUIStyle headerStyle = "AM HeaderStyle";

                GUILayout.BeginHorizontal();
                GUILayout.Label("Product Name: ", headerStyle);
                GUILayout.Label(Application.productName);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Current Product Version: ", headerStyle);
                GUILayout.Label(Application.version);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Target Build Version: ", headerStyle);
                GUILayout.Label(buildOption.nextVersion.ToString());
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Runtime Platform: ", headerStyle);
                GUILayout.Label(buildOption.platform.ToString());
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.Label("Release Notes:", headerStyle);
                GUILayout.Space(5);
                releaseNote = EditorGUILayout.TextArea(releaseNote, GUILayout.Height(240));
                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Cancel", GUILayout.Height(30)))
                {
                    Close();
                }

                if (GUILayout.Button("Build", GUILayout.Height(30)))
                {
                    // EditorApplication.delayCall += () =>
                    // {
                    //     AssetBuildUtility.BuildModules(modules, releaseNote);
                    //     Close();
                    // };
                }
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }

            private void OnDisable()
            {
                modules = null;
                releaseNote = null;
            }
        }
    }
}
