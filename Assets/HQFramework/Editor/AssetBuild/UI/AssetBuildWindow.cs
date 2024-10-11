using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

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

        protected override Task OnInitialize()
        {
            return HQAssetBuildLauncher.Initialize();
        }

        protected override void InitializeContent(out TabContentView[] contentList)
        {
            GUIContent btnBuildContent = EditorGUIUtility.IconContent("CustomTool");//BuildSettings.SelectedIcon
            btnBuildContent.text = " Assets Build";
            GUIContent btnInspectorContent = EditorGUIUtility.IconContent("d_DebuggerDisabled");
            btnInspectorContent.text = " Assets Inspector";
            GUIContent btnTableContent = EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow");
            btnTableContent.text = " Assets Table";
            GUIContent btnArchiveContent = EditorGUIUtility.IconContent("SaveAs");
            btnArchiveContent.text = " Assets Archive";
            GUIContent btnPublishContent = EditorGUIUtility.IconContent("d_CloudConnect");
            btnPublishContent.text = " Assets Publish";
            GUIContent btnConfigContent = EditorGUIUtility.IconContent("SettingsIcon");
            btnConfigContent.text = " Build Settings";
            // GUIContent btnRuntimeContent = EditorGUIUtility.IconContent("d_Profiler.Memory");

            contentList = new TabContentView[]
            {
                new AssetModuleView(this, btnBuildContent),
                new AssetTableView(this, btnTableContent),
                new AssetInspectView(this, btnInspectorContent),
                new AssetArchiveView(this, btnArchiveContent),
                new AssetPublishView(this, btnPublishContent),
                new AssetBuildConfigView(this, btnConfigContent),
            };
        }

        protected override void OnTabPanelGUI()
        {
            base.OnTabPanelGUI();
            GUILayout.FlexibleSpace();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            HQAssetBuildLauncher.Dispose();
        }
    }
}
