using System.Collections.Generic;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetModuleHotfixView : TabContentView
    {
        private GUIContent btnUIContent;
        private GUIStyle textUIStyle;
        private GUIStyle selectedBtnStyle;
        private Vector2 scrollPos;
        private AssetBuildOption buildOption;
        private List<AssetModuleConfig> hotfixModuleList;

        public AssetModuleHotfixView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override void OnEnable()
        {
            buildOption = AssetBuildOptionManager.GetDefaultOption();
            RefreshModuleList();

            if (buildOption == null || buildOption.hotfixMode == AssetHotfixMode.NoHotfix)
                return;

            btnUIContent = EditorGUIUtility.IconContent("SceneAsset Icon");
            btnUIContent.tooltip = "left click to select / deselect.\nright click to show option.";
            textUIStyle = new GUIStyle();
            textUIStyle.alignment = TextAnchor.MiddleCenter;
            textUIStyle.normal.textColor = Color.yellow;
        }

        public override void OnDisable()
        {
            hotfixModuleList.Clear();
            hotfixModuleList = null;
        }

        public override void OnGUI()
        {
            if (buildOption == null || buildOption.hotfixMode == AssetHotfixMode.NoHotfix)
                return;

            DrawModules();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            GUIContent btnBuildContent = null;
            bool enableBuild = false;
            if (buildOption == null)
            {
                btnBuildContent = EditorGUIUtility.IconContent("d_console.erroricon.sml");
                enableBuild = false;
            }
            else
            {
                switch (buildOption.platform)
                {
                    case BuildTargetPlatform.Android:
                        btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.Android.Small");
                        break;
                    case BuildTargetPlatform.iOS:
                    case BuildTargetPlatform.VisionOS:
                        btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.iPhone.Small");
                        break;
                    case BuildTargetPlatform.StandaloneOSX:
                        btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.Standalone.Small");
                        break;
                    case BuildTargetPlatform.StandaloneWindows:
                    case BuildTargetPlatform.StandaloneWindows64:
                        btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.Metro.Small");
                        break;
                    case BuildTargetPlatform.WebGL://BuildSettings.WebGL.Small
                        btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.WebGL.Small");
                        break;
                }
                enableBuild = true;
            }

            GUI.enabled = enableBuild;

            btnBuildContent.text = " Build";
            if (GUILayout.Button(btnBuildContent, GUILayout.Height(45)))
            {
                List<AssetModuleConfig> selectedModules = new List<AssetModuleConfig>();
                for (int i = 0; i < hotfixModuleList.Count; i++)
                {
                    if (hotfixModuleList[i].isBuild)
                    {
                        selectedModules.Add(hotfixModuleList[i]);
                    }
                }
                try
                {
                    AssetBuildUtility.BuildHotfixModules(selectedModules);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }

            // GUIContent btnUploadContent = EditorGUIUtility.IconContent("Update-Available");
            // btnUploadContent.text = " Upload";
            // if (GUILayout.Button(btnUploadContent, GUILayout.Height(45)))
            // {

            // }

            GUI.enabled = true;

            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        private void DrawModules()
        {
            GUILayout.Space(10);
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            scrollPos.x = 0;

            // calculate the max count per row
            int maxCountPerRow = Mathf.FloorToInt((viewRect.width - 20) / 140);

            for (int i = 0; i < hotfixModuleList.Count; i++)
            {
                if (i % maxCountPerRow == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(5);
                }

                AssetModuleConfig module = hotfixModuleList[i];
                Rect rect = GUILayoutUtility.GetRect(btnUIContent, GUI.skin.button, GUILayout.Width(140), GUILayout.Height(170));
                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.button == 1)
                    {
                        ShowContextMenu(module);
                        Event.current.Use();
                    }
                }
                if (GUI.Button(rect, btnUIContent))
                {
                    module.isBuild = !module.isBuild;
                }

                GUI.Label(new Rect(5 + i % maxCountPerRow * 148, 150 + i / maxCountPerRow * 182, 140, 20), module.name, textUIStyle);

                if (module.isBuild)
                {
                    if (selectedBtnStyle == null)
                    {
                        selectedBtnStyle = "LightmapEditorSelectedHighlight";
                        selectedBtnStyle.contentOffset = new Vector2(110, -67);
                    }

                    GUI.Toggle(new Rect(10 + i % maxCountPerRow * 148, 7 + i / maxCountPerRow * 182, 130, 160), true, EditorGUIUtility.IconContent("Collab"), selectedBtnStyle);
                }

                GUILayout.Space(5);

                if ((i + 1) % maxCountPerRow == 0 || (i + 1) == hotfixModuleList.Count)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }
            }

            GUILayout.EndScrollView();
        }

        private void ShowContextMenu(AssetModuleConfig module)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Edit"), false, () =>
            {
                EditorWindow.GetWindow<HotfixModuleEditWindow>().ShowWindow(module);
            });
            menu.ShowAsContext();
        }

        public void RefreshModuleList()
        {
            hotfixModuleList = AssetModuleManager.GetModuleList();
        }
    }
}
