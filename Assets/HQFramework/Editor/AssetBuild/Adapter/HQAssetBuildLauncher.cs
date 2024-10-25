using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    /// <summary>
    /// Main entry of asset build module in HQFramework.
    /// </summary>
    public sealed class HQAssetBuildLauncher
    {
        private static DefaultAssetConfigManager configManager = new DefaultAssetConfigManager();
        private static DefaultAssetDataManager dataManager = new DefaultAssetDataManager();

        public static AssetBuildConfig CurrentBuildConfig
        {
            get
            {
                return configManager.CurrentBuildConfig;
            }
            set
            {
                configManager.CurrentBuildConfig = value;
            }
        }

        public static async Task Initialize()
        {
            configManager.LoadAssetConfig();
            while (!configManager.Initialized)
            {
                await Task.Yield();
            }
        }

        public static async void BuildAssetModules(List<AssetModuleConfigAgent> moduleConfigAgentList)
        {
            if (moduleConfigAgentList == null || moduleConfigAgentList.Count == 0)
            {
                return;
            }

            AssetBuildConfig buildConfig = CurrentBuildConfig;
            string outputDir = Path.Combine(Application.dataPath, buildConfig.assetOutputDir);
            Type preprocessorType = Type.GetType(buildConfig.preprocessorName);
            Type compilerType = Type.GetType(buildConfig.compilerName);
            Type postprocessorType = Type.GetType(buildConfig.postprocessorName);
            IAssetBuildPreprocessor preprocessor = Activator.CreateInstance(preprocessorType) as IAssetBuildPreprocessor;
            IAssetBuildCompiler compiler = Activator.CreateInstance(compilerType) as IAssetBuildCompiler;
            IAssetBuildPostprocessor postprocessor = Activator.CreateInstance(postprocessorType) as IAssetBuildPostprocessor;

            AssetBuildController.SetPreprocessor(preprocessor);
            AssetBuildController.SetCompiler(compiler);
            AssetBuildController.SetPostprocessor(postprocessor);

            List<AssetModuleConfig> moduleConfigList = new List<AssetModuleConfig>();
            for (int i = 0; i < moduleConfigAgentList.Count; i++)
            {
                moduleConfigList.Add(GetModuleFromAgent(moduleConfigAgentList[i]));
                moduleConfigAgentList[i].buildVersionCode++;
            }

            AssetPostprocessData result = AssetBuildController.BuildAssetModules(moduleConfigList, outputDir, buildConfig.platform, buildConfig.compressOption);
            bool saved = await dataManager.AddAssetModuleCompileInfosAsync(result.dataList);
            Debug.Log(saved ? "Asset Modules Build Successfully!" : "Asset Modules Build Error..");
        }

        public static async void ArchiveAssetModules(List<AssetModuleCompileInfo> compileInfoList, string archiveTag, string archiveNotes)
        {
            if (compileInfoList == null || compileInfoList.Count == 0)
            {
                return;
            }

            AssetArchiveData archiveData = AssetArchiveController.ArchiveAssetModules(compileInfoList, archiveTag, archiveNotes);
            bool saved = await dataManager.AddAssetArchiveAsync(archiveData);
            Debug.Log(saved ? "Asset Modules Archive Successfully!" : "Asset Modules Archive Error..");
        }

        public static Task<List<AssetArchiveData>> GetAssetArchivesAsync()
        {
            return dataManager.GetAssetArchivesAsync();
        }

        public static Task<List<AssetModuleCompileInfo>> GetAssetModuleCompileHistoryAsync()
        {
            return dataManager.GetAssetModuleCompileHistoryAsync();
        }

        public static Task<List<AssetModuleManifest>> GetAssetPublishHistoryAsync()
        {
            return dataManager.GetAssetPublishHistoryAsync();
        }

        public static async void PublishAssetArchive(AssetArchiveData archiveData, string releaseNote, string resourceVersion, int versionCode, int minimalSupportedVersionCode, Dictionary<int, string> moduleReleaseNotesDic, Dictionary<int, int> moduleMinimalSupportedVersionDic, Action<int, int, string> uploadCallback, Action endCallback)
        {
            Type publishHelperType = Type.GetType(CurrentBuildConfig.publishHelperName);
            IAssetPublishHelper publishHelper = Activator.CreateInstance(publishHelperType) as IAssetPublishHelper;
            AssetPublishController.SetHelper(publishHelper);
            AssetModuleManifest result = await AssetPublishController.PublishAssets(archiveData, releaseNote, resourceVersion, versionCode, minimalSupportedVersionCode, moduleReleaseNotesDic, moduleMinimalSupportedVersionDic, uploadCallback, endCallback);
            if (result != null)
            {
                bool saved = await dataManager.AddPublishDataAsync(result);
                if (saved)
                {
                    string jsonStr = JsonUtilityEditor.ToJson(result);
                    Debug.Log($"Publish Successfully!\n{jsonStr}");
                }
            }
        }

        public static AssetBuildConfig CreateBuildConfig(string tag)
        {
            return configManager.AddBuildConfig(tag);
        }

        public static AssetModuleConfigAgent CreateModuleConfig(string moduleName, bool isBuiltin, UnityEngine.Object rootFolder, string devNotes)
        {
            return configManager.AddModuleConfig(moduleName, isBuiltin, rootFolder, devNotes);
        }

        public static bool DeleteBuildConfig(string tag)
        {
            return configManager.DeleteBuildConfig(tag);
        }

        public static bool DeleteModuleConfig(int id, bool deleteBuildHistory = false)
        {
            return configManager.DeleteModuleConfig(id, deleteBuildHistory);
        }

        public static List<AssetBuildConfig> GetBuildConfigs()
        {
            return configManager.GetBuildConfigs();
        }

        public static List<AssetModuleConfigAgent> GetModuleConfigs()
        {
            return configManager.GetModuleConfigs();
        }

        public static void Dispose()
        {
            configManager.Dispose();
            dataManager.Dispose();
        }

        private static AssetModuleConfig GetModuleFromAgent(AssetModuleConfigAgent moduleAgent)
        {
            AssetModuleConfig config = new AssetModuleConfig();
            config.id = moduleAgent.id;
            config.moduleName = moduleAgent.moduleName;
            config.buildVersionCode = moduleAgent.buildVersionCode;
            config.createTime = moduleAgent.createTime;
            config.isBuiltin = moduleAgent.isBuiltin;
            config.devNotes = moduleAgent.devNotes;
            config.bundleConfigList = new List<AssetBundleConfig>();
            string[] subFolders = AssetDatabase.GetSubFolders(AssetDatabase.GetAssetPath(moduleAgent.rootFolder));
            for (int j = 0; j < subFolders.Length; j++)
            {
                string[] assets = AssetDatabase.FindAssets("", new[] { subFolders[j] });
                List<string> assetItemList = new List<string>();
                for (int k = 0; k < assets.Length; k++)
                {
                    string filePath = AssetDatabase.GUIDToAssetPath(assets[k]);
                    if (AssetDatabase.IsValidFolder(filePath))
                    {
                        continue;
                    }
                    assetItemList.Add(filePath);
                }

                if (assetItemList.Count > 0)
                {
                    string dirName = Path.GetFileName(subFolders[j]);
                    string bundleName = $"{moduleAgent.moduleName}_{dirName}.bundle".ToLower();
                    AssetBundleConfig bundleConfig = new AssetBundleConfig();
                    bundleConfig.bundleName = bundleName;
                    bundleConfig.assetItemList = assetItemList;
                    config.bundleConfigList.Add(bundleConfig);
                }
            }
            return config;
        }
    }
}
