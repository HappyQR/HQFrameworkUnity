using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetModuleBuildView
    {
        private class HotfixEditWindow : EditorWindow
        {
            private AssetModuleConfig config;
            private GUIStyle desStyle;
            private Vector2 scrollPos;

            public void ShowWindow(AssetModuleConfig target)
            {
                config = target;
                var window = GetWindow<HotfixEditWindow>();
                window.titleContent = new GUIContent("Hotfix Edit");
                window.minSize = window.maxSize = new Vector2(480, 500);
                window.Show();
            }

            private void OnGUI()
            {
                if (desStyle == null)
                {
                    desStyle = "DD Background";
                    desStyle.contentOffset = Vector2.right * 5;
                    desStyle.padding = new RectOffset(0, 0, 5, 5);
                }

                GUILayout.BeginArea(new Rect(10, 10, position.width - 20, position.height - 20));
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                scrollPos.x = 0;

                GUIStyle headerStyle = "AM HeaderStyle";
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Module ID : {config.id}", headerStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label($"Create Time : {config.createTime:yyyy-MM-dd HH:mm:ss}");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Module Name : {config.moduleName}", headerStyle);
                GUILayout.FlexibleSpace();
                GUI.enabled = false;
                GUILayout.Label("Is Built-in:", headerStyle);
                GUILayout.Space(5);
                config.isBuiltin = GUILayout.Toggle(config.isBuiltin, "");
                GUILayout.EndHorizontal();
                GUI.enabled = true;

                GUILayout.Label($"Current Patch Version : {config.currentPatchVersion}", headerStyle);

                GUILayout.Label("Module Description:", headerStyle);

                GUILayout.Label(config.description, desStyle);
                GUILayout.Space(10);

                GUILayout.Label("Module Assets Root Folder:", headerStyle);
                GUILayout.Space(5);
                GUI.enabled = false;
                config.rootFolder = EditorGUILayout.ObjectField(GUIContent.none, config.rootFolder, typeof(DefaultAsset), false);
                GUILayout.Space(10);
                GUI.enabled = true;

                GUILayout.Label("Minimal Supported Patch Version:", headerStyle);
                GUILayout.Space(5);
                config.minimalSupportedPatchVersion = EditorGUILayout.IntField(config.minimalSupportedPatchVersion);
                GUILayout.Space(10);

                GUILayout.Label("Next Build Patch Version:", headerStyle);
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUI.enabled = !config.autoIncreasePatchVersion;
                config.nextPatchVersion = EditorGUILayout.IntField(config.nextPatchVersion);
                if (config.nextPatchVersion < config.currentPatchVersion)
                {
                    config.nextPatchVersion = config.currentPatchVersion;
                }
                GUI.enabled = true;
                GUILayout.Space(0);
                config.autoIncreasePatchVersion = GUILayout.Toggle(config.autoIncreasePatchVersion, "Auto Increase");
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.Label("Release Note:", headerStyle);
                GUILayout.Space(5);
                config.releaseNote = EditorGUILayout.TextArea(config.releaseNote, GUILayout.Height(130));
                GUILayout.Space(10);

                GUILayout.EndScrollView();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save", GUILayout.Height(35)))
                {
                    EditorUtility.SetDirty(config);
                    AssetDatabase.SaveAssetIfDirty(config);
                    Close();
                }

                GUILayout.EndArea();
            }
        }
    }
}
