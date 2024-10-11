using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetConfigManager
    {
        private class HQAssetConfig : ScriptableObject
        {
            public List<AssetBuildConfig> buildConfigList = new List<AssetBuildConfig>();
            public List<AssetModuleConfigAgent> moduleConfigList = new List<AssetModuleConfigAgent>();
        }

        private static readonly string defaultBuildConfigKey = "hqframework_default_build_config";
        private static readonly string hqAssetConfigPath = "Assets/Configuration/Editor/AssetBuild/HQAssetConfig.asset";
        private List<AssetModuleConfigAgent> moduleConfigAgentList;
        private List<AssetBuildConfig> buildConfigList;
        private string defaultBuildConfigTag;
        private AssetBuildConfig currentBuildConfig;
        private HQAssetConfig assetConfig;

        public bool Initialized => assetConfig != null;

        public AssetBuildConfig CurrentBuildConfig
        {
            get
            {
                if (currentBuildConfig == null)
                {
                    if (buildConfigList == null)
                    {
                        buildConfigList = GetBuildConfigs();
                    }
                    if (buildConfigList.Count == 0)
                    {
                        currentBuildConfig = AddBuildConfig("Default");
                    }
                    else
                    {
                        defaultBuildConfigTag = EditorPrefs.GetString(defaultBuildConfigKey, null);
                        currentBuildConfig = buildConfigList.Find((item) => item.tag == defaultBuildConfigTag);
                        if (currentBuildConfig == null)
                        {
                            currentBuildConfig = buildConfigList[0];
                        }
                    }
                }

                return currentBuildConfig;
            }
            set
            {
                currentBuildConfig = value;
                defaultBuildConfigTag = value.tag;
                EditorPrefs.SetString(defaultBuildConfigKey, defaultBuildConfigTag);
            }
        }

        public void LoadAssetConfig()
        {
            string dir = Path.GetDirectoryName(hqAssetConfigPath);
            string absDir = FileUtilityEditor.GetPhysicalPath(dir);
            if (!Directory.Exists(absDir))
            {
                Directory.CreateDirectory(absDir);
                AssetDatabase.Refresh();
            }
            if (assetConfig == null)
            {
                EditorApplication.delayCall += () =>
                {
                    assetConfig = AssetDatabase.LoadAssetAtPath<HQAssetConfig>(hqAssetConfigPath);
                    if (assetConfig == null)
                    {
                        assetConfig = ScriptableObject.CreateInstance<HQAssetConfig>();
                        AssetDatabase.CreateAsset(assetConfig, hqAssetConfigPath);
                        assetConfig = AssetDatabase.LoadAssetAtPath<HQAssetConfig>(hqAssetConfigPath);
                    }
                };
            }
        }

        public AssetBuildConfig AddBuildConfig(string tag)
        {
            if (buildConfigList == null)
            {
                buildConfigList = GetBuildConfigs();
            }

            for (int i = 0; i < buildConfigList.Count; i++)
            {
                if (buildConfigList[i].tag == tag)
                {
                    throw new InvalidOperationException($"Build Config '{tag}' is existed, you can't create it anymore.");
                }
            }

            AssetBuildConfig config = new AssetBuildConfig();
            config.assetOutputDir = "../Build/Assets";
            config.tag = tag;
            config.platform = (BuildTargetPlatform)EditorUserBuildSettings.activeBuildTarget;
            config.compressOption = CompressOption.LZ4;
            buildConfigList.Add(config);
            return config;
        }

        public AssetModuleConfigAgent AddModuleConfig(string moduleName, UnityEngine.Object rootFolder, string devNotes)
        {
            if (moduleConfigAgentList == null)
            {
                moduleConfigAgentList = GetModuleConfigs();
            }
            AssetModuleConfigAgent agent = new AssetModuleConfigAgent();
            if (moduleConfigAgentList.Count == 0)
            {
                agent.id = 0;
            }
            else
            {
                agent.id = moduleConfigAgentList[moduleConfigAgentList.Count - 1].id + 1;
            }
            agent.moduleName = moduleName;
            agent.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            agent.devNotes = devNotes;
            agent.rootFolder = rootFolder;
            agent.isBuild = true;
            moduleConfigAgentList.Add(agent);
            return agent;
        }

        public bool DeleteBuildConfig(string tag)
        {
            for (int i = 0; i < buildConfigList.Count; i++)
            {
                if (buildConfigList[i].tag == tag)
                {
                    buildConfigList.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool DeleteModuleConfig(int id, bool deleteBuildHistory)
        {
            for (int i = 0; i < moduleConfigAgentList.Count; i++)
            {
                if (moduleConfigAgentList[i].id == id)
                {
                    moduleConfigAgentList.RemoveAt(i);
                    if (deleteBuildHistory)
                    {
                        DeleteModuleBuildData(id);
                    }
                    return true;
                }
            }
            return false;
        }

        public List<AssetBuildConfig> GetBuildConfigs()
        {
            if (buildConfigList == null)
            {
                buildConfigList = assetConfig.buildConfigList;
            }
            return buildConfigList;
        }

        public List<AssetModuleConfigAgent> GetModuleConfigs()
        {
            if (moduleConfigAgentList == null)
            {
                moduleConfigAgentList = assetConfig.moduleConfigList;
            }
            return moduleConfigAgentList;
        }

        public void Dispose()
        {
            EditorUtility.SetDirty(assetConfig);
            AssetDatabase.SaveAssetIfDirty(assetConfig);

            assetConfig = null;
            currentBuildConfig = null;
            moduleConfigAgentList = null;
            buildConfigList = null;
        }

        private void DeleteModuleBuildData(int moduleID)
        {

        }
    }
}
