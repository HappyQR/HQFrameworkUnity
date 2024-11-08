using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetBuildConfigView : TabContentView
    {
        private List<AssetBuildConfig> configList;
        private AssetBuildConfig currentBuildConfig;
        private string[] configTagList;
        private int selectedConfigIndex;
        private int previousSelectedConfigIndex;
        private string[] preprocessorTypeList;
        private int selectedPreprocessorTypeIndex;
        private string[] compilerTypeList;
        private int selectedCompilerTypeIndex;
        private string[] postprocessorTypeList;
        private int selectedPostprocessorTypeIndex;
        private string[] publishHelperTypeList;
        private int selectedPublishHelperTypeIndex;
        private string[] assetUploaderTypeList;
        private int selectedAssetUploaderTypeIndex;
        private Vector2 scrollPos;

        public AssetBuildConfigView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override void OnEnable()
        {
            previousSelectedConfigIndex = -1;
            currentBuildConfig = HQAssetBuildLauncher.CurrentBuildConfig;
            configList = HQAssetBuildLauncher.GetBuildConfigs();
            configTagList = new string[configList.Count + 1];
            for (int i = 0; i < configList.Count; i++)
            {
                configTagList[i] = configList[i].tag;
                if (currentBuildConfig == configList[i])
                {
                    selectedConfigIndex = i;
                    previousSelectedConfigIndex = i;
                }
            }
            configTagList[configTagList.Length - 1] = "Add New...";
            CollectBuildActors();
            OnSelectConfig();
        }
        
        public override void OnGUI()
        {
            GUIStyle headerStyle = "AM HeaderStyle";
            GUILayout.BeginArea(new Rect(10, 10, viewRect.width - 20, viewRect.height - 20));
            GUILayout.BeginHorizontal("PreBackground");

            GUILayout.Label("Build Option Tag: ", headerStyle);
            selectedConfigIndex = EditorGUILayout.Popup(selectedConfigIndex, configTagList);
            if (selectedConfigIndex == configTagList.Length - 1)
            {
                PopupNewOption();
                selectedConfigIndex = previousSelectedConfigIndex;
            }
            else if (previousSelectedConfigIndex != selectedConfigIndex)
            {
                currentBuildConfig = configList[selectedConfigIndex];
                HQAssetBuildLauncher.CurrentBuildConfig = currentBuildConfig;
                previousSelectedConfigIndex = selectedConfigIndex;
                OnSelectConfig();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            scrollPos.x = 0;

            GUILayout.Label("Asset Output Dir(Related to Application.dataPath):", headerStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            currentBuildConfig.assetOutputDir = EditorGUILayout.TextField(currentBuildConfig.assetOutputDir);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string absDir = EditorUtility.OpenFolderPanel("Choose a directory to save assets:", Application.dataPath + "/..", "");
                if (!string.IsNullOrEmpty(absDir))
                {
                    string relatedDir = Path.GetRelativePath(Application.dataPath, absDir);
                    currentBuildConfig.assetOutputDir = relatedDir.Replace("\\", "/");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Asset Built-in Dir(Related to Application.streamingAssetsPath):", headerStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            currentBuildConfig.assetBuiltinDir = EditorGUILayout.TextField(currentBuildConfig.assetBuiltinDir);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string absDir = EditorUtility.OpenFolderPanel("Choose a directory to save assets:", Application.streamingAssetsPath, "");
                if (!string.IsNullOrEmpty(absDir))
                {
                    string relatedDir = Path.GetRelativePath(Application.streamingAssetsPath, absDir);
                    currentBuildConfig.assetBuiltinDir = relatedDir.Replace("\\", "/");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("Select a Asset Build Preprocessor:", headerStyle);
            GUILayout.Space(5);
            selectedPreprocessorTypeIndex = EditorGUILayout.Popup(selectedPreprocessorTypeIndex, preprocessorTypeList);
            if (preprocessorTypeList.Length > 0)
            {
                currentBuildConfig.preprocessorName = preprocessorTypeList[selectedPreprocessorTypeIndex];
            }
            GUILayout.Space(10);

            GUILayout.Label("Select a Asset Build Compiler:", headerStyle);
            GUILayout.Space(5);
            selectedCompilerTypeIndex = EditorGUILayout.Popup(selectedCompilerTypeIndex, compilerTypeList);
            if (compilerTypeList.Length > 0)
            {
                currentBuildConfig.compilerName = compilerTypeList[selectedCompilerTypeIndex];
            }
            GUILayout.Space(10);

            GUILayout.Label("Select a Asset Build Postprocessor:", headerStyle);
            GUILayout.Space(5);
            selectedPostprocessorTypeIndex = EditorGUILayout.Popup(selectedPostprocessorTypeIndex, postprocessorTypeList);
            if (postprocessorTypeList.Length > 0)
            {
                currentBuildConfig.postprocessorName = postprocessorTypeList[selectedPostprocessorTypeIndex];
            }
            GUILayout.Space(10);

            GUILayout.Label("Select a Compression Function:", headerStyle);
            GUILayout.Space(5);
            currentBuildConfig.compressOption = (CompressOption)EditorGUILayout.EnumPopup(currentBuildConfig.compressOption);
            GUILayout.Space(10);

            GUILayout.Label("Select a Target Platform:", headerStyle);
            GUILayout.Space(5);
            currentBuildConfig.platform = (BuildTargetPlatform)EditorGUILayout.EnumPopup(currentBuildConfig.platform);
            EditorGUILayout.Space(10);

            GUILayout.Label("Select a Asset Publish Helper:", headerStyle);
            GUILayout.Space(5);
            selectedPublishHelperTypeIndex = EditorGUILayout.Popup(selectedPublishHelperTypeIndex, publishHelperTypeList);
            if (publishHelperTypeList.Length > 0)
            {
                currentBuildConfig.publishHelperName = publishHelperTypeList[selectedPublishHelperTypeIndex];
            }
            GUILayout.Space(10);

            GUILayout.Label("Select a Asset Uploader:", headerStyle);
            GUILayout.Space(5);
            selectedAssetUploaderTypeIndex = EditorGUILayout.Popup(selectedAssetUploaderTypeIndex, assetUploaderTypeList);
            if (assetUploaderTypeList.Length > 0)
            {
                currentBuildConfig.assetUploaderName = assetUploaderTypeList[selectedAssetUploaderTypeIndex];
            }
            GUILayout.Space(10);

            GUILayout.Label("Assets Hotfix URL Root Folder:", headerStyle);
            GUILayout.Space(5);
            currentBuildConfig.hotfixRootFolder = EditorGUILayout.TextField(currentBuildConfig.hotfixRootFolder);
            GUILayout.Space(10);

            GUILayout.Label("Assets Hotfix Manifest File Name:", headerStyle);
            GUILayout.Space(5);
            currentBuildConfig.hotfixManifestFileName = EditorGUILayout.TextField(currentBuildConfig.hotfixManifestFileName);
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
            GUILayout.EndArea();
        }

        private void OnSelectConfig()
        {
            for (int i = 0; i < preprocessorTypeList.Length; i++)
            {
                if (preprocessorTypeList[i] == currentBuildConfig.preprocessorName)
                {
                    selectedPreprocessorTypeIndex = i;
                    break;
                }
            }

            for (int i = 0; i < compilerTypeList.Length; i++)
            {
                if (compilerTypeList[i] == currentBuildConfig.compilerName)
                {
                    selectedCompilerTypeIndex = i;
                    break;
                }
            }

            for (int i = 0; i < postprocessorTypeList.Length; i++)
            {
                if (postprocessorTypeList[i] == currentBuildConfig.postprocessorName)
                {
                    selectedPostprocessorTypeIndex = i;
                    break;
                }
            }

            for (int i = 0; i < publishHelperTypeList.Length; i++)
            {
                if (publishHelperTypeList[i] == currentBuildConfig.publishHelperName)
                {
                    selectedPublishHelperTypeIndex = i;
                    break;
                }
            }

            for (int i = 0; i < assetUploaderTypeList.Length; i++)
            {
                if (assetUploaderTypeList[i] == currentBuildConfig.assetUploaderName)
                {
                    selectedAssetUploaderTypeIndex = i;
                    break;
                }
            }
        }

        private void PopupNewOption()
        {
            CreateNewWindow.ShowWindow(this);
        }

        private void AddNewBuildConfig(AssetBuildConfig config)
        {
            currentBuildConfig = config;
            configList = HQAssetBuildLauncher.GetBuildConfigs();
            HQAssetBuildLauncher.CurrentBuildConfig = config;
            configTagList = new string[configList.Count + 1];
            for (int i = 0; i < configList.Count; i++)
            {
                configTagList[i] = configList[i].tag;
                if (currentBuildConfig == configList[i])
                {
                    selectedConfigIndex = i;
                    previousSelectedConfigIndex = i;
                }
            }
            configTagList[configTagList.Length - 1] = "Add New";
            OnSelectConfig();
        }

        private void CollectBuildActors()
        {
            List<string> preprocessorTypes = new List<string>();
            List<string> postprocessorTypes = new List<string>();
            List<string> compilerTypes = new List<string>();
            List<string> publishHelperTypes = new List<string>();
            List<string> uploaderTypes = new List<string>();

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
                        else if (typeof(IAssetPublishHelper).IsAssignableFrom(types[j]))
                        {
                            publishHelperTypes.Add(types[j].FullName);
                        }
                        else if (typeof(IAssetUploader).IsAssignableFrom(types[j]))
                        {
                            uploaderTypes.Add(types[j].FullName);
                        }
                    }
                }
            }

            preprocessorTypeList = preprocessorTypes.ToArray();
            compilerTypeList = compilerTypes.ToArray();
            postprocessorTypeList = postprocessorTypes.ToArray();
            publishHelperTypeList = publishHelperTypes.ToArray();
            assetUploaderTypeList = uploaderTypes.ToArray();
        }

        public override void OnDisable()
        {
            configList = null;
            configTagList = null;
            preprocessorTypeList = null;
            compilerTypeList = null;
            postprocessorTypeList = null;
        }
    }
}
