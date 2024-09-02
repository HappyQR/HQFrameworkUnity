using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System;

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

            btnBuildContent.text = " Build";
            if (GUILayout.Button(btnBuildContent, GUILayout.Height(45), GUILayout.Width((viewRect.width - 20) / 2)))
            {
                EditorWindow.GetWindow<AssetModuleBuildWindow>().ShowWindow(moduleList);
            }
            GUIContent btnCheckContent = EditorGUIUtility.IconContent("d_DebuggerDisabled");
            btnCheckContent.text = " Inspect";
            if (GUILayout.Button(btnCheckContent, GUILayout.Height(45), GUILayout.Width((viewRect.width - 20) / 2)))
            {
                EditorApplication.delayCall += AssetBuildUtility.InspectAssetModules;
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
                    EditorWindow.GetWindow<AssetModuleEditWindow>().ShowWindow(null, OnCreateNewModule);
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
                    // module.isBuild = !module.isBuild;
                }

                GUI.Label(new Rect(5 + i % maxCountPerRow * 148, 150 + i / maxCountPerRow * 182, 140, 20), module.name, textUIStyle);

                if (selectedBtnStyle == null)
                {
                    selectedBtnStyle = "LightmapEditorSelectedHighlight";
                    selectedBtnStyle.contentOffset = new Vector2(110, -67);
                }

                GUI.Toggle(new Rect(10 + i % maxCountPerRow * 148, 7 + i / maxCountPerRow * 182, 130, 160), true, EditorGUIUtility.IconContent("Collab"), selectedBtnStyle);

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
                            EditorWindow.GetWindow<AssetModuleEditWindow>().ShowWindow(null, OnCreateNewModule);
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        // draw add button at the end
                        if (GUILayout.Button(btnAddContent, GUILayout.Width(140), GUILayout.Height(170)))
                        {
                            EditorWindow.GetWindow<AssetModuleEditWindow>().ShowWindow(null, OnCreateNewModule);
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
                EditorWindow.GetWindow<AssetModuleEditWindow>().ShowWindow(module, null);
            });
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

    public class AssetModuleEditWindow : EditorWindow
    {
        private AssetModuleConfig config;
        private Action<AssetModuleConfig> createCallback;
        private bool createNewConfig;
        private bool saveNewConfig;

        private static readonly string tempDir = "Assets/Configuration/Editor/Asset/AssetModule/";

        public void ShowWindow(AssetModuleConfig target, Action<AssetModuleConfig> createCallback)
        {
            config = target;
            this.createCallback = createCallback;
            if (config == null)
            {
                int id = AssetModuleConfigManager.GetNewModuleID();
                AssetModuleConfig temp = ScriptableObject.CreateInstance<AssetModuleConfig>();
                temp.id = id;
                temp.currentPatchVersion = 1;
                temp.minimalSupportedPatchVersion = 1;
                string tempPath = tempDir + "temp.asset";
                AssetDatabase.CreateAsset(temp, tempPath);
                config = AssetDatabase.LoadAssetAtPath<AssetModuleConfig>(tempPath);
                createNewConfig = true;
            }
            var window = GetWindow<AssetModuleEditWindow>();
            window.minSize = window.maxSize = new Vector2(480, 360);
            window.titleContent = new GUIContent("Create New Asset Module");
            window.Show();
        }

        private void OnDisable()
        {
            if (createNewConfig && !saveNewConfig)
            {
                AssetDatabase.DeleteAsset(tempDir + "temp.asset");
            }
            else
            {
                if (config != null)
                {
                    EditorUtility.SetDirty(config);
                    AssetDatabase.SaveAssetIfDirty(config);
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, position.width - 20, position.height - 20));

            GUIStyle headerStyle = "AM HeaderStyle";
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Module ID : {config.id}", headerStyle);
            if (!createNewConfig)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label($"Create Time : {config.createTime:yyyy-MM-dd HH:mm:ss}");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Module Name:", headerStyle);
            GUILayout.Space(5);
            config.moduleName = GUILayout.TextField(config.moduleName);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Is Built-in:", headerStyle);
            GUILayout.Space(5);
            config.isBuiltin = GUILayout.Toggle(config.isBuiltin, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Module Assets Root Folder:", headerStyle);
            GUILayout.Space(5);
            config.rootFolder = EditorGUILayout.ObjectField(GUIContent.none, config.rootFolder, typeof(DefaultAsset), false);
            GUILayout.Space(10);

            GUILayout.Label("Module Description:", headerStyle);
            GUILayout.Space(5);
            config.description = EditorGUILayout.TextArea(config.description, GUILayout.Height(120));
            GUILayout.FlexibleSpace();

            if (createNewConfig)
            {
                if (GUILayout.Button("Create New Module"))
                {
                    saveNewConfig = true;
                    // Create New Module
                    if (AssetModuleConfigManager.CreateNewAssetModule(config))
                    {
                        createCallback?.Invoke(config);
                        Close();
                    }
                }
            }

            GUILayout.EndArea();
        }
    }

    public class AssetModuleBuildWindow : EditorWindow
    {
        private List<AssetModuleConfig> modules;
        private string releaseNote;
        private Vector2 scrollPos;

        public void ShowWindow(List<AssetModuleConfig> modules)
        {
            this.modules = modules;
            AssetModuleBuildWindow window = GetWindow<AssetModuleBuildWindow>();
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

            GUILayout.Label("Include Modules:", headerStyle);
            GUILayout.Space(5);
            for (int i = 0; i < modules.Count; i++)
            {
                GUILayout.Label(modules[i].moduleName, "AssetLabel");
                GUILayout.Space(3);
            }
            GUILayout.Space(30);

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
                EditorApplication.delayCall += () => 
                {
                    AssetBuildUtility.BuildAssetModules(modules, releaseNote);
                    Close();
                };
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
