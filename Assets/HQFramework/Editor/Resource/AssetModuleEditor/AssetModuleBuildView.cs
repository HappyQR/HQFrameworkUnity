using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace HQFramework.Editor
{
    public class AssetModuleBuildView : TabContentView
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
            buildOption = AssetBuildOptionManager.GetDefaultOption();
            btnUIContent = EditorGUIUtility.IconContent("SceneAsset Icon");
            btnAddContent = EditorGUIUtility.IconContent("CollabCreate Icon");
            btnAddContent.tooltip = "click to add a new module";
            btnUIContent.tooltip = "left click to select / deselect.\nright click to show option.";
            textUIStyle = new GUIStyle();
            textUIStyle.alignment = TextAnchor.MiddleCenter;
            textUIStyle.normal.textColor = Color.yellow;
            RefreshModuleList();
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

            btnBuildContent.text = " Build";
            if (GUILayout.Button(btnBuildContent, GUILayout.Height(45)))
            {
                List<AssetModuleConfig> selectedModules = new List<AssetModuleConfig>();
                StringBuilder existPatchModuleContent = new StringBuilder();
                for (int i = 0; i < moduleList.Count; i++)
                {
                    if (moduleList[i].isBuild)
                    {
                        selectedModules.Add(moduleList[i]);
                        if (moduleList[i].currentPatchVersion > 1)
                        {
                            existPatchModuleContent.Append(moduleList[i].moduleName);
                            existPatchModuleContent.Append(" : ");
                            existPatchModuleContent.Append("Current Patch Version:");
                            existPatchModuleContent.Append(moduleList[i].currentPatchVersion);
                            existPatchModuleContent.Append("\n");
                        }
                    }
                }
                if (existPatchModuleContent.Length > 0)
                {
                    bool flag = EditorUtility.DisplayDialog("Generic Asset Build Warnning",
                                                "You select modules exist new patch version, this operation will clear the hotfixes of these modules, are you sure to continue? \n\n" + existPatchModuleContent.ToString(),
                                                "Yes, Continue",
                                                "No, Cancel");
                    if (flag)
                    {
                        try
                        {
                            AssetBuildUtility.BuildModules(selectedModules);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError(ex.Message);
                        }
                    }
                }
                else
                {
                    try
                    {
                        AssetBuildUtility.BuildModules(selectedModules);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }
            }

            // GUIContent btnUploadContent = EditorGUIUtility.IconContent("Update-Available");
            // btnUploadContent.text = " Upgrade";
            // if (GUILayout.Button(btnUploadContent, GUILayout.Height(45)))
            // {
            //     if (moduleList.Count == 0)
            //         return;
            //     bool result = EditorUtility.DisplayDialog($"Upgrade Assets Generic Version: {buildOption.resourceVersion}->{buildOption.resourceVersion + 1}",
            //                                     "This operation will rebuild all assets module, clear the hotfix version and overide AssetModuleManifest.\n\nAre you sure to continue?",
            //                                     "Yes, Upgrade.",
            //                                     "No, Cancel");
            //     if (result)
            //     {
            //         AssetBuildUtility.UpgradeAssetModuleGenericVersion();
            //         EditorWindow.GetWindow<AssetBuildWindow>().Repaint();
            //     }
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
                    EditorWindow.GetWindow<AssetModuleEditWindow>().ShowWindow(null);
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
                            EditorWindow.GetWindow<AssetModuleEditWindow>().ShowWindow(null);
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        // draw add button at the end
                        if (GUILayout.Button(btnAddContent, GUILayout.Width(140), GUILayout.Height(170)))
                        {
                            EditorWindow.GetWindow<AssetModuleEditWindow>().ShowWindow(null);
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
                EditorWindow.GetWindow<AssetModuleEditWindow>().ShowWindow(module);
            });
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                if (AssetModuleManager.DeleteAssetModule(module))
                {
                    RefreshModuleList();
                }
            });
            menu.ShowAsContext();
        }

        public void RefreshModuleList()
        {
            moduleList = AssetModuleManager.GetModuleList();
        }
    }
}
