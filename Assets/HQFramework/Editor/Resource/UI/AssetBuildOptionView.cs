using System;
using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetBuildOptionView : TabContentView
    {
        private List<AssetBuildOption> optionList;
        private string[] optionTagList;
        private AssetBuildOption buildOption;
        private Vector2 scrollPos;
        private int selectedOptionIndex;
        private int previousSelectedOptionIndex;

        public AssetBuildOptionView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override void OnEnable()
        {
            previousSelectedOptionIndex = -1;
            optionList = AssetBuildOptionManager.GetConfigList();
            buildOption = AssetBuildOptionManager.GetDefaultConfig();
            optionTagList = new string[optionList.Count + 1];
            for (int i = 0; i < optionList.Count; i++)
            {
                optionTagList[i] = optionList[i].optionTag;
                if (buildOption == optionList[i])
                {
                    selectedOptionIndex = i;
                    previousSelectedOptionIndex = i;
                }
            }
            optionTagList[optionTagList.Length - 1] = "Add New...";
        }
        
        public override void OnGUI()
        {
            if (buildOption == null && optionList.Count == 0)
            {
                GUILayout.Space(viewRect.height / 2 - 30);
                if (GUILayout.Button("Create New Build Option"))
                {
                    PopupNewOption();
                }
                return;
            }

            GUIStyle headerStyle = "AM HeaderStyle";
            GUILayout.BeginArea(new Rect(10, 10, viewRect.width - 20, viewRect.height - 20));
            GUILayout.BeginHorizontal("PreBackground");

            GUILayout.Label("Build Option Tag: ", headerStyle);
            selectedOptionIndex = EditorGUILayout.Popup(selectedOptionIndex, optionTagList);
            if (selectedOptionIndex == optionTagList.Length - 1)
            {
                PopupNewOption();
                selectedOptionIndex = previousSelectedOptionIndex;
            }
            else if (previousSelectedOptionIndex != selectedOptionIndex)
            {
                buildOption = optionList[selectedOptionIndex];
                AssetBuildOptionManager.SetDefaultConfig(buildOption);
                previousSelectedOptionIndex = selectedOptionIndex;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (buildOption == null)
            {
                GUILayout.EndArea();
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            scrollPos.x = 0;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Product Name: ", headerStyle);
            GUILayout.Label(Application.productName);
            GUILayout.FlexibleSpace();
            // GUILayout.Label($"Build Tag : {buildOption.optionTag}", "AssetLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Product Version: ", headerStyle);
            GUILayout.Label(Application.version);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Resource Version: ", headerStyle);
            GUILayout.Label(buildOption.resourceVersion.ToString());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Runtime Platform: ", headerStyle);
            GUILayout.Label(buildOption.platform.ToString());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hotfix Mode:", headerStyle);
            GUILayout.Space(5);
            buildOption.hotfixMode = (AssetHotfixMode)EditorGUILayout.EnumPopup(buildOption.hotfixMode);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Asset Output Dir(Related to Application.dataPath):", headerStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            buildOption.bundleOutputDir = EditorGUILayout.TextField(buildOption.bundleOutputDir);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string absDir = EditorUtility.OpenFolderPanel("Choose a directory to save assets:", Application.dataPath + "/..", "");
                if (!string.IsNullOrEmpty(absDir))
                {
                    string relatedDir = Path.GetRelativePath(Application.dataPath, absDir);
                    buildOption.bundleOutputDir = relatedDir.Replace("\\", "/");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Asset Built-in Dir(Related to Application.streamingAssetsPath):", headerStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            buildOption.builtinDir = EditorGUILayout.TextField(buildOption.builtinDir);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string absDir = EditorUtility.OpenFolderPanel("Choose a built-in directory:", Application.streamingAssetsPath, "");
                if (!string.IsNullOrEmpty(absDir))
                {
                    string relatedDir = Path.GetRelativePath(Application.streamingAssetsPath, absDir);
                    buildOption.builtinDir = relatedDir.Replace("\\", "/");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Select a Compression Function:", headerStyle);
            GUILayout.Space(5);
            buildOption.compressOption = (CompressOption)EditorGUILayout.EnumPopup(buildOption.compressOption);
            GUILayout.Space(10);

            GUILayout.Label("Select a Target Platform:", headerStyle);
            GUILayout.Space(5);
            buildOption.platform = (BuildTargetPlatform)EditorGUILayout.EnumPopup(buildOption.platform);
            EditorGUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Next Build Resource Version:", headerStyle);
            GUILayout.Space(5);
            GUI.enabled = !buildOption.autoIncreaseResourceVersion;
            buildOption.nextVersion = EditorGUILayout.IntField(buildOption.nextVersion, GUILayout.Width(100));
            if (buildOption.nextVersion < buildOption.resourceVersion)
            {
                buildOption.nextVersion = buildOption.resourceVersion;
            }
            GUI.enabled = true;
            GUILayout.Space(5);
            buildOption.autoIncreaseResourceVersion = GUILayout.Toggle(buildOption.autoIncreaseResourceVersion, "Auto Increase");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Minimal Supported Resource Version:", headerStyle);
            GUILayout.Space(5);
            buildOption.minimalSupportedVersion = EditorGUILayout.IntField(buildOption.minimalSupportedVersion, GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Enable Bundle Encryption:", headerStyle);
            // GUILayout.Space(5);
            // buildOption.enableEncryption = GUILayout.Toggle(buildOption.enableEncryption, "");
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();
            // GUILayout.Space(10);

            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();

            // GUILayout.BeginHorizontal();
            

            // GUIContent btnUploadContent = EditorGUIUtility.IconContent("d_RotateTool On");
            // btnUploadContent.text = " Sync With Server";
            // if (GUILayout.Button(btnUploadContent, GUILayout.Height(45), GUILayout.Width((viewRect.width - 30) / 2)))
            // {
            //     EditorApplication.delayCall += AssetsUploader.SyncAssetsWithServer;
            // }
            // GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void PopupNewOption()
        {
            CreateNewOptionWindow.Show((tag) =>
            {
                buildOption = AssetBuildOptionManager.CreateNewConfig(tag);
                optionList = AssetBuildOptionManager.GetConfigList();
                AssetBuildOptionManager.SetDefaultConfig(buildOption);
                optionTagList = new string[optionList.Count + 1];
                for (int i = 0; i < optionList.Count; i++)
                {
                    optionTagList[i] = optionList[i].optionTag;
                    if (buildOption == optionList[i])
                    {
                        selectedOptionIndex = i;
                        previousSelectedOptionIndex = i;
                    }
                }
                optionTagList[optionTagList.Length - 1] = "Add New";
            });
        }

        public override void OnDisable()
        {
            optionList = null;
            optionTagList = null;
            if (buildOption != null)
            {
                EditorUtility.SetDirty(buildOption);
                AssetDatabase.SaveAssetIfDirty(buildOption);
            }
        }
    }


    public class CreateNewOptionWindow : EditorWindow
    {
        private string tag;
        private static Action<string> confirmCallback;

        public static void Show(Action<string> callback)
        {
            confirmCallback = callback;
            var window = GetWindow<CreateNewOptionWindow>();
            window.titleContent = new GUIContent("Create Build Option");
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
            GUILayout.Label("Option Tag : ");
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
