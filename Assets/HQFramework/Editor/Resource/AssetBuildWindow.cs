using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HQFramework.Editor
{
    public class AssetBuildWindow : TabContentWindow
    {
        private AssetBuildOption buildOption;

        [MenuItem("HQFramework/Build/Assets Build")]
        private static void ShowWindow()
        {
            var window = GetWindow<AssetBuildWindow>();
            window.titleContent = new GUIContent("Assets Builder");
            window.minSize = new Vector2(675, 520);
            window.Show();
        }

        protected override void OnInitialized(out List<TabContentView> contentList)
        {
            GUIContent btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.SelectedIcon");
            btnBuildContent.text = " Build Modules";
            GUIContent btnHotfixContent = EditorGUIUtility.IconContent("CustomTool");
            btnHotfixContent.text = " Build Hotfixes";
            GUIContent btnConfigContent = EditorGUIUtility.IconContent("SettingsIcon");
            btnConfigContent.text = " Build Settings";
            GUIContent btnRuntimeContent = EditorGUIUtility.IconContent("d_Profiler.Memory");
            btnRuntimeContent.text = " Runtime Settings";

            buildOption = AssetBuildUtility.GetDefaultOption();

            contentList = new List<TabContentView>
            {
                new AssetModuleBuildView(this, btnBuildContent),
                new AssetModuleHotfixView(this, btnHotfixContent),
                new AssetBuildOptionView(this, btnConfigContent),
                new AssetRuntimeSettingView(this, btnRuntimeContent),
            };
        }

        protected override void OnTabPanelGUI()
        {
            base.OnTabPanelGUI();
            GUILayout.FlexibleSpace();

            if (buildOption != null)
            {
                GUILayout.Label($"Assets Generic Version : {buildOption.genericVersion}", "WhiteLargeCenterLabel");
                GUILayout.Space(20);
            }
        }

        public void RefreshModuleList()
        {
            if (currentContentView is AssetModuleBuildView)
            {
                (currentContentView as AssetModuleBuildView).RefreshModuleList();
            }
            else if (currentContentView is AssetModuleHotfixView)
            {
                (currentContentView as AssetModuleHotfixView).RefreshModuleList();
            }
        }
    }
}