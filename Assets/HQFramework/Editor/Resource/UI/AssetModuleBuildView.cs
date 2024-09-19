using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HQFramework.Editor
{
    public partial class AssetModuleBuildView : TabContentView
    {
        private GUIContent btnUIContent;
        private GUIContent btnAddContent;
        private GUIStyle textUIStyle;
        private GUIStyle selectedBtnStyle;
        private Vector2 scrollPos;
        private AssetBuildOption buildOption;
        private List<AssetModuleConfig> moduleList;

        public AssetModuleBuildView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override void OnEnable()
        {
            buildOption = AssetBuildOptionManager.GetDefaultConfig();
            btnUIContent = EditorGUIUtility.IconContent("SceneAsset Icon");
            btnAddContent = EditorGUIUtility.IconContent("CollabCreate Icon");
            btnAddContent.tooltip = "click to add a new module";
            btnUIContent.tooltip = "left click to select / deselect.\nright click to show option.";
            textUIStyle = new GUIStyle();
            textUIStyle.alignment = TextAnchor.MiddleCenter;
            textUIStyle.normal.textColor = Color.yellow;
            moduleList = AssetModuleConfigManager.GetModuleList();
        }

        public override void OnDisable()
        {
            moduleList.Clear();
            moduleList = null;
        }

        public override void OnGUI()
        {
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
                    EditorApplication.delayCall += AssetBuildUtility.ClearBuildHistory;
                }
            }

            btnBuildContent.text = " Build Assets";
            if (GUILayout.Button(btnBuildContent, GUILayout.Height(45), GUILayout.Width((viewRect.width - 20) / 2)))
            {
                List<AssetModuleConfig> buildList = new List<AssetModuleConfig>();
                bool includeBuiltin = false;
                for (int i = 0; i < moduleList.Count; i++)
                {
                    if (moduleList[i].isBuild)
                    {
                        if (moduleList[i].isBuiltin)
                        {
                            includeBuiltin = true;
                        }
                        buildList.Add(moduleList[i]);
                    }
                }
                // if (buildOption.hotfixMode == Resource.AssetHotfixMode.SeparateHotfix && !includeBuiltin)
                // {
                //     EditorApplication.delayCall += () => AssetBuildUtility.BuildModulesWithoutBuiltin(buildList);
                // }
                // else
                // {
                //     EditorWindow.GetWindow<ConfirmWindow>().ShowWindow(buildList, buildOption);
                // }
            }
            // GUIContent btnCheckContent = EditorGUIUtility.IconContent("");
            // btnCheckContent.text = " Inspect";
            // if (GUILayout.Button(btnCheckContent, GUILayout.Height(45), GUILayout.Width((viewRect.width - 20) / 2)))
            // {
            //     EditorApplication.delayCall += AssetBuildUtility.InspectAssetModules;
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
            if (moduleList.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                if (GUILayout.Button(btnAddContent, GUILayout.Width(140), GUILayout.Height(170)))
                {
                    EditorWindow.GetWindow<ModuleEditWindow>().ShowWindow(null, OnCreateNewModule);
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

                AssetModuleConfig module = moduleList[i];
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

                if (selectedBtnStyle == null)
                {
                    selectedBtnStyle = "LightmapEditorSelectedHighlight";
                    selectedBtnStyle.contentOffset = new Vector2(110, -67);
                }

                if (module.isBuild)
                {
                    GUI.Toggle(new Rect(10 + i % maxCountPerRow * 148, 7 + i / maxCountPerRow * 182, 130, 160), true, EditorGUIUtility.IconContent("Collab"), selectedBtnStyle);
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
                            EditorWindow.GetWindow<ModuleEditWindow>().ShowWindow(null, OnCreateNewModule);
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        // draw add button at the end
                        if (GUILayout.Button(btnAddContent, GUILayout.Width(140), GUILayout.Height(170)))
                        {
                            EditorWindow.GetWindow<ModuleEditWindow>().ShowWindow(null, OnCreateNewModule);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        private void ShowContextMenu(AssetModuleConfig module)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Edit"), false, () =>
            {
                EditorWindow.GetWindow<ModuleEditWindow>().ShowWindow(module, null);
            });
            if (buildOption.hotfixMode == Resource.AssetHotfixMode.SeparateHotfix && !module.isBuiltin)
            {
                menu.AddItem(new GUIContent("Hotfix"), false, () =>
                {
                    EditorWindow.GetWindow<HotfixEditWindow>().ShowWindow(module);
                });
            }
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                if (AssetModuleConfigManager.DeleteAssetModule(module))
                {
                    moduleList.Remove(module);
                }
            });
            menu.ShowAsContext();
        }

        private void OnCreateNewModule(AssetModuleConfig module)
        {
            moduleList.Add(module);
        }
    }
}
