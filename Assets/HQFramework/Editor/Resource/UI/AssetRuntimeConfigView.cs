using System;
using System.Collections.Generic;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetRuntimeConfigView : TabContentView
    {
        private List<AssetRuntimeConfig> configList;
        private string[] configTagList;
        private AssetRuntimeConfig config;
        private Vector2 scrollPos;
        private int selectedConfigIndex;
        private int previousSelectedConfigIndex;

        public AssetRuntimeConfigView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override void OnEnable()
        {
            previousSelectedConfigIndex = -1;
            configList = AssetRuntimeConfigManager.GetConfigList();
            config = AssetRuntimeConfigManager.GetDefaultConfig();
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
        }

        public override void OnGUI()
        {
            if (config == null && configList.Count == 0)
            {
                GUILayout.Space(viewRect.height / 2 - 30);
                if (GUILayout.Button("Create New Runtime Config"))
                {
                    PopupNewConfig();
                }
                return;
            }

            GUIStyle headerStyle = "AM HeaderStyle";
            GUILayout.BeginArea(new Rect(10, 10, viewRect.width - 20, viewRect.height - 20));
            GUILayout.BeginHorizontal("PreBackground");

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
                AssetRuntimeConfigManager.SetDefaultConfig(config);
                previousSelectedConfigIndex = selectedConfigIndex;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (config == null)
            {
                GUILayout.EndArea();
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            scrollPos.x = 0;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hotfix Mode:", headerStyle);
            GUILayout.Space(5);
            config.hotfixMode = (AssetHotfixMode)EditorGUILayout.EnumPopup(config.hotfixMode);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Asset Built-in Dir(Related to Application.streamingAssetsPath):", headerStyle);
            GUILayout.Space(5);
            config.assetBuiltinDir = EditorGUILayout.TextField(config.assetBuiltinDir);
            GUILayout.Space(10);

            GUILayout.Label("Asset Persistent Dir(Related to Application.persistentDataPath):", headerStyle);
            GUILayout.Space(5);
            config.assetPersistentDir = EditorGUILayout.TextField(config.assetPersistentDir);
            GUILayout.Space(10);

            if (config.hotfixMode != AssetHotfixMode.NoHotfix)
            {
                GUILayout.Label("Hotfix Manifest URL:", headerStyle);
                GUILayout.Space(5);
                config.hotfixManifestUrl = EditorGUILayout.TextField(config.hotfixManifestUrl);
                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();

            GUIContent content = EditorGUIUtility.IconContent("d_editicon.sml");
            content.text = " Generate Assets Runtime Settings";
            if (GUILayout.Button(content, GUILayout.Height(45)))
            {
                EditorApplication.delayCall += () => AssetRuntimeConfigManager.GenerateRuntimeSettings(config);
            }

            GUILayout.EndArea();
        }

        public override void OnDisable()
        {
            configList = null;
            configTagList = null;
            if (config != null)
            {
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssetIfDirty(config);
            }
        }

        private void PopupNewConfig()
        {
            CreateNewAssetRuntimeConfigWindow.Show((tag) =>
            {
                config = AssetRuntimeConfigManager.CreateNewConfig(tag);
                configList = AssetRuntimeConfigManager.GetConfigList();
                AssetRuntimeConfigManager.SetDefaultConfig(config);
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
    }

    public class CreateNewAssetRuntimeConfigWindow : EditorWindow
    {
        private string tag;
        private static Action<string> confirmCallback;

        public static void Show(Action<string> callback)
        {
            confirmCallback = callback;
            var window = GetWindow<CreateNewAssetRuntimeConfigWindow>();
            window.titleContent = new GUIContent("Create Asset Config");
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
