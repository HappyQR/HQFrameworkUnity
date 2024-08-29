using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{   
    public class AppBuildWindow : EditorWindow
    {
        private List<AppBuildConfig> configList;
        private string[] configTagList;
        private AppBuildConfig config;
        private Vector2 scrollPos;
        private Rect viewRect;
        private int selectedConfigIndex;
        private int previousSelectedConfigIndex;
        private Texture2D appIcon;

        [MenuItem("HQFramework/Build/App Build")]
        private static void ShowWindow()
        {
            var window = GetWindow<AppBuildWindow>();
            window.titleContent = new GUIContent("HQ App Build");
            window.minSize = window.maxSize = new Vector2(600, 600);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.delayCall += OnInit;
        }

        private void OnInit()
        {
            viewRect = new Rect(0, 0, 600, 600);
            previousSelectedConfigIndex = -1;
            configList = AppBuildConfigManager.GetConfigList();
            config = AppBuildConfigManager.GetDefaultConfig();
            configTagList = new string[configList.Count + 1];
            for (int i = 0; i < configList.Count; i++)
            {
                configTagList[i] = configList[i].tag;
                if (config == configList[i])
                {
                    selectedConfigIndex = i;
                    previousSelectedConfigIndex = i;
                }
            }
            configTagList[configTagList.Length - 1] = "Add New...";

            Texture2D[] icons = PlayerSettings.GetIconsForTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (icons != null && icons.Length > 0)
            {
                appIcon = icons[0];
            }
        }
    
        private void OnGUI()
        {
            if (config == null && (configList == null || configList.Count == 0))
            {
                GUILayout.Space(viewRect.height / 2 - 30);
                if (GUILayout.Button("Create New App Build Config"))
                {
                    PopupNewConfig();
                }
                return;
            }

            GUIStyle headerStyle = "AM HeaderStyle";
            GUILayout.BeginArea(new Rect(10, 10, viewRect.width - 20, viewRect.height - 20));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Runtime Config Tag: ", headerStyle);
            selectedConfigIndex = EditorGUILayout.Popup(selectedConfigIndex, configTagList);
            if (selectedConfigIndex == configTagList.Length - 1)
            {
                PopupNewConfig();
                selectedConfigIndex = previousSelectedConfigIndex;
            }
            else if (previousSelectedConfigIndex != selectedConfigIndex)
            {
                config = configList[selectedConfigIndex];
                AppBuildConfigManager.SetDefaultConfig(config);
                previousSelectedConfigIndex = selectedConfigIndex;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (appIcon != null)
            {
                GUI.DrawTexture(new Rect(480, 30, 80, 80), appIcon, ScaleMode.ScaleToFit);
            }

            if (config == null)
            {
                GUILayout.EndArea();
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            scrollPos.x = 0;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Product Name: ", headerStyle);
            config.productName = Application.productName;
            GUILayout.Label(config.productName);
            GUILayout.FlexibleSpace();
            // GUILayout.Label($"Version Tag : {config.versionTag}", "AssetLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Product Version: ", headerStyle);
            config.productVersion = Application.version;
            GUILayout.Label(config.productVersion);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Company Name: ", headerStyle);
            config.companyName = Application.companyName;
            GUILayout.Label(config.companyName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Runtime Platform: ", headerStyle);
            config.runtimePlatform = EditorUserBuildSettings.activeBuildTarget.ToString();
            GUILayout.Label(config.runtimePlatform);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Version Tag:", headerStyle);
            GUILayout.Space(5);
            config.versionTag = EditorGUILayout.TextField(config.versionTag, GUILayout.Width(100));
            GUI.enabled = true;
            GUILayout.Space(5);
            config.devBuild = GUILayout.Toggle(config.devBuild, "DEV BUILD");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Build File Name:", headerStyle);
            GUILayout.Space(5);
            config.buildBundleName = EditorGUILayout.TextField(config.buildBundleName);
            GUILayout.Space(10);

            GUILayout.Label("Build Output Dir(Related to Application.dataPath):", headerStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            config.bundleOutputDir = EditorGUILayout.TextField(config.bundleOutputDir);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string absDir = EditorUtility.OpenFolderPanel("Choose a directory to save bundle:", Application.dataPath + "/..", "");
                if (!string.IsNullOrEmpty(absDir))
                {
                    string relatedDir = Path.GetRelativePath(Application.dataPath, absDir);
                    config.bundleOutputDir = relatedDir.Replace("\\", "/");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Internal Version:", headerStyle);
            GUILayout.Space(5);
            GUI.enabled = !config.autoIncreaseBuildVersion;
            config.internalVersionCode = EditorGUILayout.IntField(config.internalVersionCode, GUILayout.Width(100));
            GUI.enabled = true;
            GUILayout.Space(5);
            config.autoIncreaseBuildVersion = GUILayout.Toggle(config.autoIncreaseBuildVersion, "Auto Increase");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Minimal Supported Version:", headerStyle);
            GUILayout.Space(5);
            config.minimalSupportedVersionCode = EditorGUILayout.IntField(config.minimalSupportedVersionCode, GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Release Note:", headerStyle);
            GUILayout.Space(5);
            config.releaseNote = EditorGUILayout.TextArea(config.releaseNote, GUILayout.Height(160));
            GUILayout.Space(10);

            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build App Bundle", GUILayout.Height(45)))
            {
                EditorApplication.delayCall += () => AppBuildUtility.StartBuild(config);
            }

            if (GUILayout.Button("Upload Version Info", GUILayout.Height(45)))
            {

            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void PopupNewConfig()
        {
            CreateNewAppBuildConfigWindow.Show((tag) =>
            {
                config = AppBuildConfigManager.CreateNewConfig(tag);
                configList = AppBuildConfigManager.GetConfigList();
                AppBuildConfigManager.SetDefaultConfig(config);
                configTagList = new string[configList.Count + 1];
                for (int i = 0; i < configList.Count; i++)
                {
                    configTagList[i] = configList[i].tag;
                    if (config == configList[i])
                    {
                        selectedConfigIndex = i;
                        previousSelectedConfigIndex = i;
                    }
                }
                configTagList[configTagList.Length - 1] = "Add New";
            });
        }

        private void OnDisable()
        {
            configList = null;
            configTagList = null;
            appIcon = null;
            if (config != null)
            {
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssetIfDirty(config);

                AppBuildUtility.GenerateVersionInfo(config);
            }
        }
    }

    public class CreateNewAppBuildConfigWindow : EditorWindow
    {
        private string tag;
        private static Action<string> confirmCallback;

        public static void Show(Action<string> callback)
        {
            confirmCallback = callback;
            var window = GetWindow<CreateNewAppBuildConfigWindow>();
            window.titleContent = new GUIContent("Create App Build Config");
            window.maxSize = new Vector2(270, 100);
            window.Show();
        }

        void OnDestroy()
        {
            confirmCallback = null;
        }

        private void OnGUI()
        {
            GUILayout.Space(25);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("Config Tag : ");
            tag = EditorGUILayout.TextField(tag);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            if (GUILayout.Button("Confirm"))
            {
                if (string.IsNullOrEmpty(tag))
                {
                    Debug.LogError("You need to enter a tag!");
                    return;
                }

                confirmCallback.Invoke(tag);
                Close();
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
    }
}
