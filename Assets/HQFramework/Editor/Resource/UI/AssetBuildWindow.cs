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
            GUIContent btnBuildContent = EditorGUIUtility.IconContent("CustomTool");//BuildSettings.SelectedIcon
            btnBuildContent.text = " Assets Build";
            GUIContent btnInspectorContent = EditorGUIUtility.IconContent("d_DebuggerDisabled");
            btnInspectorContent.text = " Assets Inspector";
            GUIContent btnTableContent = EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow");
            btnTableContent.text = " Assets Table";
            GUIContent btnConfigContent = EditorGUIUtility.IconContent("SettingsIcon");
            btnConfigContent.text = " Build Settings";
            GUIContent btnRuntimeContent = EditorGUIUtility.IconContent("d_Profiler.Memory");
            btnRuntimeContent.text = " Runtime Settings";
            GUIContent btnArchiveContent = EditorGUIUtility.IconContent("SaveAs");
            btnArchiveContent.text = " Assets Archive";
            GUIContent btnPublishContent = EditorGUIUtility.IconContent("d_CloudConnect");
            btnPublishContent.text = " Assets Publish";

            contentList = new TabContentView[]
            {
                new AssetModuleBuildView(this, btnBuildContent),
                new AssetInspectorView(this, btnInspectorContent),
                new AssetCrcTableView(this, btnTableContent),
                new AssetBuildOptionView(this, btnConfigContent),
                new AssetRuntimeConfigView(this, btnRuntimeContent),
                new AssetArchiveView(this, btnArchiveContent),
                new AssetPublishView(this, btnPublishContent)
            };
        }

        protected override void OnTabPanelGUI()
        {
            base.OnTabPanelGUI();
            GUILayout.FlexibleSpace();
        }
    }
}