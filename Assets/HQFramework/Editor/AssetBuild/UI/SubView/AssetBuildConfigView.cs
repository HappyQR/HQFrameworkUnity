using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetBuildConfigView : TabContentView
    {
        private List<AssetBuildConfig> optionList;
        private AssetBuildConfig buildOption;
        private string[] optionTagList;
        private int selectedOptionIndex;
        private int previousSelectedOptionIndex;
        private string[] preprocessorTypeList;
        private int selectedPreprocessorTypeIndex;
        private string[] compilerTypeList;
        private int selectedCompilerTypeIndex;
        private string[] postprocessorTypeList;
        private int selectedPostprocessorTypeIndex;
        private Vector2 scrollPos;

        public AssetBuildConfigView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override void OnEnable()
        {
            previousSelectedOptionIndex = -1;
            optionList = AssetBuildConfig.GetConfigList();
            buildOption = AssetBuildConfig.Default;
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

            CollectBuildActors();
            OnSelectConfig();
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
                AssetBuildConfig.Default = buildOption;
                previousSelectedOptionIndex = selectedOptionIndex;
                OnSelectConfig();
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

            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Product Name: ", headerStyle);
            // GUILayout.Label(Application.productName);
            // GUILayout.FlexibleSpace();
            // // GUILayout.Label($"Build Tag : {buildOption.optionTag}", "AssetLabel");
            // GUILayout.EndHorizontal();

            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Current Product Version: ", headerStyle);
            // GUILayout.Label(Application.version);
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();

            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Runtime Platform: ", headerStyle);
            // GUILayout.Label(buildOption.platform.ToString());
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();
            // GUILayout.Space(10);

            GUILayout.Label("Asset Output Dir(Related to Application.dataPath):", headerStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            buildOption.assetOutputDir = EditorGUILayout.TextField(buildOption.assetOutputDir);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string absDir = EditorUtility.OpenFolderPanel("Choose a directory to save assets:", Application.dataPath + "/..", "");
                if (!string.IsNullOrEmpty(absDir))
                {
                    string relatedDir = Path.GetRelativePath(Application.dataPath, absDir);
                    buildOption.assetOutputDir = relatedDir.Replace("\\", "/");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Select a Asset Build Preprocessor:", headerStyle);
            GUILayout.Space(5);
            selectedPreprocessorTypeIndex = EditorGUILayout.Popup(selectedPreprocessorTypeIndex, preprocessorTypeList);
            if (preprocessorTypeList.Length > 0)
            {
                buildOption.preprocessorName = preprocessorTypeList[selectedPreprocessorTypeIndex];
            }
            GUILayout.Space(10);

            GUILayout.Label("Select a Asset Build Compiler:", headerStyle);
            GUILayout.Space(5);
            selectedCompilerTypeIndex = EditorGUILayout.Popup(selectedCompilerTypeIndex, compilerTypeList);
            if (compilerTypeList.Length > 0)
            {
                buildOption.compilerName = compilerTypeList[selectedCompilerTypeIndex];
            }
            GUILayout.Space(10);

            GUILayout.Label("Select a Asset Build Postprocessor:", headerStyle);
            GUILayout.Space(5);
            selectedPostprocessorTypeIndex = EditorGUILayout.Popup(selectedPostprocessorTypeIndex, postprocessorTypeList);
            if (postprocessorTypeList.Length > 0)
            {
                buildOption.postprocessorName = postprocessorTypeList[selectedPostprocessorTypeIndex];
            }
            GUILayout.Space(10);

            GUILayout.Label("Select a Compression Function:", headerStyle);
            GUILayout.Space(5);
            buildOption.compressOption = (CompressOption)EditorGUILayout.EnumPopup(buildOption.compressOption);
            GUILayout.Space(10);

            GUILayout.Label("Select a Target Platform:", headerStyle);
            GUILayout.Space(5);
            buildOption.platform = (BuildTargetPlatform)EditorGUILayout.EnumPopup(buildOption.platform);
            EditorGUILayout.Space(10);

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

        private void OnSelectConfig()
        {
            for (int i = 0; i < preprocessorTypeList.Length; i++)
            {
                if (preprocessorTypeList[i] == buildOption.preprocessorName)
                {
                    selectedPreprocessorTypeIndex = i;
                    break;
                }
            }

            for (int i = 0; i < compilerTypeList.Length; i++)
            {
                if (compilerTypeList[i] == buildOption.compilerName)
                {
                    selectedCompilerTypeIndex = i;
                    break;
                }
            }

            for (int i = 0; i < postprocessorTypeList.Length; i++)
            {
                if (postprocessorTypeList[i] == buildOption.postprocessorName)
                {
                    selectedPostprocessorTypeIndex = i;
                    break;
                }
            }
        }

        private void PopupNewOption()
        {
            CreateNewWindow.Show((tag) =>
            {
                buildOption = AssetBuildConfig.CreateNewConfig(tag);
                optionList = AssetBuildConfig.GetConfigList();
                AssetBuildConfig.Default = buildOption;
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

        private void CollectBuildActors()
        {
            List<string> preprocessorTypes = new List<string>();
            List<string> postprocessorTypes = new List<string>();
            List<string> compilerTypes = new List<string>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    if (!types[j].IsAbstract && !types[j].IsInterface)
                    {
                        if (typeof(IAssetBuildPreprocessor).IsAssignableFrom(types[j]))
                        {
                            preprocessorTypes.Add(types[j].FullName);
                        }
                        else if (typeof(IAssetBuildCompiler).IsAssignableFrom(types[j]))
                        {
                            compilerTypes.Add(types[j].FullName);
                        }
                        else if (typeof(IAssetBuildPostprocessor).IsAssignableFrom(types[j]))
                        {
                            postprocessorTypes.Add(types[j].FullName);
                        }
                    }
                }
            }

            preprocessorTypeList = preprocessorTypes.ToArray();
            compilerTypeList = compilerTypes.ToArray();
            postprocessorTypeList = postprocessorTypes.ToArray();
        }

        public override void OnDisable()
        {
            optionList = null;
            optionTagList = null;
            preprocessorTypeList = null;
            compilerTypeList = null;
            postprocessorTypeList = null;
            if (buildOption != null)
            {
                EditorUtility.SetDirty(buildOption);
                AssetDatabase.SaveAssetIfDirty(buildOption);
            }
        }
    }
}
