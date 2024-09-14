using UnityEngine;
using UnityEditor;

namespace HQFramework.Editor
{
    public class AssetBuildWindow : TabContentWindow
    {
        [MenuItem("HQFramework/Build/Assets Build")]
        private static void ShowWindow()
        {
            var window = GetWindow<AssetBuildWindow>();
            window.titleContent = new GUIContent("HQ Assets Build");
            window.minSize = new Vector2(675, 520);
            window.Show();
        }

        protected override void OnInitialized(out TabContentView[] contentList)
        {
            GUIContent btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.SelectedIcon");
            btnBuildContent.text = " Assets Build";
            GUIContent btnInspectorContent = EditorGUIUtility.IconContent("CustomTool");
            btnInspectorContent.text = " Assets Inspector";
            GUIContent btnTableContent = EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow");
            btnTableContent.text = " Assets Table";
            GUIContent btnConfigContent = EditorGUIUtility.IconContent("SettingsIcon");
            btnConfigContent.text = " Build Settings";
            GUIContent btnRuntimeContent = EditorGUIUtility.IconContent("d_Profiler.Memory");
            btnRuntimeContent.text = " Runtime Settings";
            GUIContent btnUploadContent = EditorGUIUtility.IconContent("d_CloudConnect");
            btnUploadContent.text = " Assets Upload";

            contentList = new TabContentView[]
            {
                new AssetModuleBuildView(this, btnBuildContent),
                new AssetInspectorView(this, btnInspectorContent),
                new AssetTableView(this, btnTableContent),
                new AssetBuildOptionView(this, btnConfigContent),
                new AssetRuntimeConfigView(this, btnRuntimeContent),
                new AssetUploadView(this, btnUploadContent)
            };
        }

        protected override void OnTabPanelGUI()
        {
            base.OnTabPanelGUI();
            GUILayout.FlexibleSpace();
        }
    }
}