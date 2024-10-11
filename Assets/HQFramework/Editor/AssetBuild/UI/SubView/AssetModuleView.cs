using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetModuleView : TabContentView
    {
        private GUIContent btnModuleContent;
        private GUIContent btnAddContent;
        private GUIStyle textModuleNameStyle;
        private GUIStyle selectedModuleStyle;
        private Vector2 scrollPos;
        private List<AssetModuleConfigAgent> moduleList;

        public AssetModuleView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override void OnEnable()
        {
            btnModuleContent = EditorGUIUtility.IconContent("SceneAsset Icon");
            btnAddContent = EditorGUIUtility.IconContent("CollabCreate Icon");
            btnAddContent.tooltip = "click to add a new module";
            btnModuleContent.tooltip = "left click to select / deselect.\nright click to show option.";
            textModuleNameStyle = new GUIStyle();
            textModuleNameStyle.alignment = TextAnchor.MiddleCenter;
            textModuleNameStyle.normal.textColor = Color.yellow;

            moduleList = HQAssetBuildLauncher.GetModuleConfigs();
        }

        public override void OnGUI()
        {
            DrawModules();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUIContent btnBuildContent = GetBuildBtnContent();
            if (btnBuildContent == null)
            {
                btnBuildContent = EditorGUIUtility.IconContent("d_console.erroricon.sml");
                GUI.enabled = false;
            }
            GUIContent btnCheckContent = EditorGUIUtility.IconContent("TreeEditor.Trash");
            btnCheckContent.text = " Clear Builds";
            if (GUILayout.Button(btnCheckContent, GUILayout.Height(45), GUILayout.Width((viewRect.width - 20) / 2)))
            {
                bool result = EditorUtility.DisplayDialog("Delete All Asset Build History",
                                                "This operation will delete all assets builds, Are you sure to continue?",
                                                "Yes, Delete.",
                                                "No, Cancel");
                if (result)
                {
                    // TODO: delete build history
                }
            }

            btnBuildContent.text = " Build Assets";
            if (GUILayout.Button(btnBuildContent, GUILayout.Height(45), GUILayout.Width((viewRect.width - 20) / 2)))
            {
                List<AssetModuleConfigAgent> buildList = new List<AssetModuleConfigAgent>();
                for (int i = 0; i < moduleList.Count; i++)
                {
                    if (moduleList[i].isBuild)
                    {
                        buildList.Add(moduleList[i]);
                    }
                }
                EditorApplication.delayCall += () => HQAssetBuildLauncher.BuildAssetModules(buildList);
            }

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
            if (moduleList.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                if (GUILayout.Button(btnAddContent, GUILayout.Width(140), GUILayout.Height(170)))
                {
                    OpenModuleCreateWindow();
                }
                GUILayout.EndHorizontal();
            }

            // calculate the max count per row
            int maxCountPerRow = Mathf.FloorToInt((viewRect.width - 20) / 140);

            for (int i = 0; i < moduleList.Count; i++)
            {
                if (i % maxCountPerRow == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(5);
                }

                AssetModuleConfigAgent module = moduleList[i];
                Rect rect = GUILayoutUtility.GetRect(btnModuleContent, GUI.skin.button, GUILayout.Width(140), GUILayout.Height(170));
                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.button == 1)
                    {
                        ShowContextMenu(module);
                        Event.current.Use();
                    }
                }
                if (GUI.Button(rect, btnModuleContent))
                {
                    module.isBuild = !module.isBuild;
                }

                GUI.Label(new Rect(5 + i % maxCountPerRow * 148, 150 + i / maxCountPerRow * 182, 140, 20), module.moduleName, textModuleNameStyle);

                if (selectedModuleStyle == null)
                {
                    selectedModuleStyle = "LightmapEditorSelectedHighlight";
                    selectedModuleStyle.contentOffset = new Vector2(110, -67);
                }

                if (module.isBuild)
                {
                    GUI.Toggle(new Rect(10 + i % maxCountPerRow * 148, 7 + i / maxCountPerRow * 182, 130, 160), true, EditorGUIUtility.IconContent("Collab"), selectedModuleStyle);
                }

                GUILayout.Space(5);

                if ((i + 1) % maxCountPerRow == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }

                if ((i + 1) == moduleList.Count)
                {
                    if ((i + 1) % maxCountPerRow == 0)
                    {
                        // draw add button at next line
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(5);
                        if (GUILayout.Button(btnAddContent, GUILayout.Width(140), GUILayout.Height(170)))
                        {
                            OpenModuleCreateWindow();
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        // draw add button at the end
                        if (GUILayout.Button(btnAddContent, GUILayout.Width(140), GUILayout.Height(170)))
                        {
                            OpenModuleCreateWindow();
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        private void ShowContextMenu(AssetModuleConfigAgent module)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Edit"), false, () =>
            {
                OpenModuleEditWindow(module);
            });
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                if (HQAssetBuildLauncher.DeleteModuleConfig(module.id))
                {
                    moduleList.Remove(module);
                }
            });
            menu.ShowAsContext();
        }

        private void OpenModuleEditWindow(AssetModuleConfigAgent module)
        {
            EditorApplication.delayCall += () => ModuleEditWindow.ShowWindow(module);
        }

        private void OpenModuleCreateWindow()
        {
            EditorApplication.delayCall += () => CreateNewWindow.ShowWindow(this);
        }

        private GUIContent GetBuildBtnContent()
        {
            GUIContent btnBuildContent = null;

            if (HQAssetBuildLauncher.CurrentBuildConfig == null)
            {
                return null;
            }
            switch (HQAssetBuildLauncher.CurrentBuildConfig.platform)
            {
                case BuildTargetPlatform.Android:
                    btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.Android.Small");
                    break;
                case BuildTargetPlatform.iOS:
                    btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.iPhone.Small");
                    break;
                case BuildTargetPlatform.VisionOS:
                    btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.VisionOS.Small");
                    break;
                case BuildTargetPlatform.StandaloneOSX:
                    btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.Standalone.Small");
                    break;
                case BuildTargetPlatform.StandaloneWindows:
                case BuildTargetPlatform.StandaloneWindows64:
                    btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.Metro.Small");
                    break;
                case BuildTargetPlatform.WebGL:
                    btnBuildContent = EditorGUIUtility.IconContent("BuildSettings.WebGL.Small");
                    break;
            }
            return btnBuildContent;
        }

        public override void OnDisable()
        {
            moduleList = null;
        }
    }
}
