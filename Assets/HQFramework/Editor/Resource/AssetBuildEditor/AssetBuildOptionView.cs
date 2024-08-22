using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using HQFramework.Resource;

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
            optionList = AssetBuildOptionManager.GetOptionList();
            buildOption = AssetBuildOptionManager.GetDefaultOption();
            optionTagList = new string[optionList.Count + 1];
            for (int i = 0; i < optionList.Count; i++)
            {
                optionTagList[i] = optionList[i].tag;
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

            GUILayout.Label("Option Tag: ", headerStyle);
            selectedOptionIndex = EditorGUILayout.Popup(selectedOptionIndex, optionTagList);
            if (selectedOptionIndex == optionTagList.Length - 1)
            {
                PopupNewOption();
                selectedOptionIndex = previousSelectedOptionIndex;
            }
            else if (previousSelectedOptionIndex != selectedOptionIndex)
            {
                buildOption = optionList[selectedOptionIndex];
                AssetBuildOptionManager.SetDefaultOption(buildOption);
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
            GUILayout.Label("Hotfix Mode:", headerStyle);
            GUILayout.Space(5);
            buildOption.hotfixMode = (AssetHotfixMode)EditorGUILayout.EnumPopup(buildOption.hotfixMode);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Assets Module Generic Version : {buildOption.resourceVersion}", "AssetLabel");
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Enable Bundle Encryption:", headerStyle);
            // GUILayout.Space(5);
            // buildOption.enableEncryption = GUILayout.Toggle(buildOption.enableEncryption, "");
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();
            // GUILayout.Space(10);

            GUILayout.Label("AssetBundle Output Dir:", headerStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            buildOption.bundleOutputDir = EditorGUILayout.TextField(buildOption.bundleOutputDir);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                buildOption.bundleOutputDir = EditorUtility.OpenFolderPanel("Choose a directory to save bundles:", Application.dataPath + "/..", "");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("AssetBundle Manifest Output Dir:", headerStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            buildOption.manifestOutputDir = EditorGUILayout.TextField(buildOption.manifestOutputDir);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                buildOption.manifestOutputDir = EditorUtility.OpenFolderPanel("Choose a directory to save manifest:", Application.dataPath + "/..", "");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.Label("AssetBundle Built-in Dir:", headerStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            buildOption.builtinDir = EditorGUILayout.TextField(buildOption.builtinDir);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                buildOption.builtinDir = EditorUtility.OpenFolderPanel("Choose a directory to save manifest:", Application.streamingAssetsPath, "");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (buildOption.hotfixMode != AssetHotfixMode.NoHotfix)
            {
                GUILayout.Label("AssetBundle Upload URL:", headerStyle);
                GUILayout.Space(5);
                buildOption.bundleUploadUrl = EditorGUILayout.TextField(buildOption.bundleUploadUrl);

                GUILayout.Space(10);

                GUILayout.Label("AssetBundle Manifest Upload URL:", headerStyle);
                GUILayout.Space(5);
                buildOption.manifestUploadUrl = EditorGUILayout.TextField(buildOption.manifestUploadUrl);

                GUILayout.Space(10);
            }

            GUILayout.Label("Select a Compression Function:", headerStyle);
            GUILayout.Space(5);
            buildOption.compressOption = (CompressOption)EditorGUILayout.EnumPopup(buildOption.compressOption);

            GUILayout.Space(10);

            GUILayout.Label("Select a Target Platform:", headerStyle);
            GUILayout.Space(5);
            buildOption.platform = (BuildTargetPlatform)EditorGUILayout.EnumPopup(buildOption.platform);

            EditorGUILayout.Space(10);

            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUIContent btnCheckContent = EditorGUIUtility.IconContent("d_DebuggerDisabled");
            btnCheckContent.text = " Check All Modules";
            if (GUILayout.Button(btnCheckContent, GUILayout.Height(45)))
            {
                
            }

            GUIContent btnUploadContent = EditorGUIUtility.IconContent("d_RotateTool On");
            btnUploadContent.text = " Sync With Server";
            if (GUILayout.Button(btnUploadContent, GUILayout.Height(45)))
            {
                
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void PopupNewOption()
        {
            CreateNewOptionWindow.Show((tag) =>
            {
                buildOption = AssetBuildOptionManager.CreateNewOption(tag);
                optionList = AssetBuildOptionManager.GetOptionList();
                AssetBuildOptionManager.SetDefaultOption(buildOption);
                optionTagList = new string[optionList.Count + 1];
                for (int i = 0; i < optionList.Count; i++)
                {
                    optionTagList[i] = optionList[i].tag;
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
            window.maxSize = new Vector2(250, 85);
            window.Show();
        }

        void OnDestroy()
        {
            confirmCallback = null;
        }

        private void OnGUI()
        {
            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("Tag : ");
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