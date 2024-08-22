using System;
using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetBuildUtility
    {
        public enum AssetBuildType
        {
            Generic,
            Hotfix
        }

        private static readonly string bundleBuildOptionPrefsKey = "bundleOption";
        private static readonly string buildOptionDir = "Assets/Config/EditorConfig/Build/";
        private static readonly string assetsModuleConfigDir = "Assets/Config/EditorConfig/AssetModule/";
        public static readonly string manifestFileName = "AssetModuleManifest.json";

        public static void BuildAllModules()
        {
            List<AssetModuleConfig> modules = GetModuleList();
            BuildModules(modules);
        }

        public static void BuildModules(List<AssetModuleConfig> modules)
        {
            Dictionary<int, AssetModuleInfo> moduleDic = new Dictionary<int, AssetModuleInfo>(modules.Count);
            for (var i = 0; i < modules.Count; i++)
            {
                moduleDic.Add(modules[i].id, BuildModule(modules[i], AssetBuildType.Generic));
            }

            GenerateAssetsManifest(moduleDic);
        }

        public static void BuildHotfixModules(List<AssetModuleConfig> modules)
        {
            Dictionary<int, AssetModuleInfo> moduleDic = new Dictionary<int, AssetModuleInfo>(modules.Count);
            for (var i = 0; i < modules.Count; i++)
            {
                moduleDic.Add(modules[i].id, BuildModule(modules[i], AssetBuildType.Hotfix));
            }

            GenerateAssetsManifest(moduleDic);
        }

        public static AssetModuleInfo BuildModule(AssetModuleConfig module, AssetBuildType buildType)
        {
            AssetBundleBuild[] builds = InitializeModuleBuild(module);
            AssetBuildOption buildOption = GetDefaultOption();
            string bundleOutputDir = Path.Combine(buildOption.bundleOutputDir,
                                                  buildOption.genericVersion.ToString(),
                                                  module.moduleName);
            if (!Directory.Exists(bundleOutputDir))
            {
                Directory.CreateDirectory(bundleOutputDir);
            }
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(bundleOutputDir,
                                                                           builds,
                                                                           (BuildAssetBundleOptions)buildOption.compressOption,
                                                                           (BuildTarget)buildOption.platform);
            if (manifest == null)
            {
                throw new ArgumentNullException("Empty Build.");
            }
            string[] bundles = manifest.GetAllAssetBundles();
            HashSet<string> bundleSet = new HashSet<string>();
            AssetModuleInfo moduleInfo = new AssetModuleInfo();
            moduleInfo.id = module.id;
            moduleInfo.moduleName = module.moduleName;
            moduleInfo.rootVersion = buildOption.genericVersion;
            moduleInfo.description = module.description;
            moduleInfo.isBuiltin = module.isBuiltin;
            switch (buildType)
            {
                case AssetBuildType.Generic:
                    module.currentPatchVersion = 1;
                    module.minimalSupportedPatchVersion = 1;
                    module.releaseNote = "";
                    break;
                case AssetBuildType.Hotfix:
                    if (module.autoIncreasePatchVersion)
                    {
                        module.currentPatchVersion++;
                    }
                    break;
            }
            moduleInfo.currentPatchVersion = module.currentPatchVersion;
            moduleInfo.minimalSupportedPatchVersion = module.minimalSupportedPatchVersion;
            moduleInfo.releaseNote = module.releaseNote;
            moduleInfo.bundleDic = new Dictionary<string, AssetBundleInfo>(bundles.Length);
            moduleInfo.assetsDic = new Dictionary<uint, AssetItemInfo>();
            for (int i = 0; i < bundles.Length; i++)
            {
                string bundleFilePath = Path.Combine(bundleOutputDir, bundles[i]);
                bundleSet.Add(bundleFilePath);
                AssetBundleInfo bundleInfo = new AssetBundleInfo();
                bundleInfo.moduleID = module.id;
                bundleInfo.bundleName = bundles[i];
                bundleInfo.dependencies = manifest.GetAllDependencies(bundles[i]);
                bundleInfo.md5 = FileUtilityEditor.GetMD5(bundleFilePath);
                bundleInfo.size = FileUtilityEditor.GetFileSize(bundleFilePath);
                moduleInfo.bundleDic.Add(bundleInfo.bundleName, bundleInfo);
                string[] assetsPathArr = AssetDatabase.GetAssetPathsFromAssetBundle(bundles[i]);
                for (int j = 0; j < assetsPathArr.Length; j++)
                {
                    AssetItemInfo assetItem = new AssetItemInfo();
                    assetItem.assetPath = assetsPathArr[j];
                    assetItem.assetName = Path.GetFileName(assetsPathArr[j]);
                    assetItem.bundleName = bundles[i];
                    assetItem.moduleID = module.id;
                    assetItem.crc = Utility.CRC32.ComputeCrc32(assetsPathArr[j]);
                    moduleInfo.assetsDic.Add(assetItem.crc, assetItem);
                }
            }

            string[] files = Directory.GetFiles(bundleOutputDir);
            for (int i = 0; i < files.Length; i++)
            {
                if (!bundleSet.Contains(files[i]))
                {
                    File.Delete(files[i]);
                }
            }

            EditorUtility.SetDirty(module);
            AssetDatabase.SaveAssetIfDirty(module);

            if (module.isBuiltin)
            {
                string desDir = Path.Combine(buildOption.builtinDir, module.moduleName);
                if (Directory.Exists(desDir))
                    Directory.Delete(desDir, true);
                Directory.CreateDirectory(desDir);
                foreach (var item in bundleSet)
                {
                    string desPath = Path.Combine(desDir, Path.GetFileName(item));
                    File.Copy(item, desPath, true);
                }
                AssetDatabase.Refresh();
            }

            return moduleInfo;
        }

        private static AssetBundleBuild[] InitializeModuleBuild(AssetModuleConfig module)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            string[] subFolders = AssetDatabase.GetSubFolders(AssetDatabase.GetAssetPath(module.rootFolder));
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            for (int i = 0; i < subFolders.Length; i++)
            {
                // Step1: Find all assets under the sub folder and set the bundle name
                // Step2: Collect the AssetBundleBuild objects
                string dirName = Path.GetFileName(subFolders[i]);
                string bundleName = $"{module.moduleName}_{dirName}.bundle".ToLower();

                string[] assets = AssetDatabase.FindAssets("", new[] { subFolders[i] });
                for (int j = 0; j < assets.Length; j++)
                {
                    string filePath = AssetDatabase.GUIDToAssetPath(assets[j]);
                    AssetImporter importer = AssetImporter.GetAtPath(filePath);
                    if (importer != null)
                    {
                        importer.assetBundleName = bundleName;
                    }
                }

                assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                if (assets != null && assets.Length > 0)
                {
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = bundleName;
                    build.assetNames = assets;
                    builds.Add(build);
                }
            }
            return builds.ToArray();
        }

        public static void GenerateAssetsManifest(Dictionary<int, AssetModuleInfo> moduleInfoDic)
        {
            AssetBuildOption buildOption = GetDefaultOption();
            Dictionary<int, AssetModuleInfo> builtinModuleInfoDic = new Dictionary<int, AssetModuleInfo>();
            (AssetModuleManifest moduleManifest, AssetModuleManifest builtinManifest) = GetCurrentAssetsManifest();
            if (moduleManifest == null)
            {
                moduleManifest = new AssetModuleManifest();
            }
            if (builtinManifest == null)
            {
                builtinManifest = new AssetModuleManifest();
            }
            // moduleManifest.genericVersion = buildOption.genericVersion;
            // builtinManifest.genericVersion = buildOption.genericVersion;
            if (moduleManifest.moduleDic == null || moduleManifest.moduleDic.Count == 0)
            {
                moduleManifest.moduleDic = moduleInfoDic;
                foreach (var item in moduleInfoDic)
                {
                    if (item.Value.isBuiltin)
                    {
                        builtinModuleInfoDic.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                foreach (var item in moduleInfoDic)
                {
                    if (!moduleManifest.moduleDic.ContainsKey(item.Key))
                    {
                        moduleManifest.moduleDic.Add(item.Key, item.Value);
                    }
                    else
                    {
                        moduleManifest.moduleDic[item.Key] = item.Value;
                    }

                    if (item.Value.isBuiltin)
                    {
                        builtinModuleInfoDic.Add(item.Key, item.Value);
                    }
                }
            }
            string manifestInfo = JsonUtilityEditor.ToJson(moduleManifest);
            Debug.Log(manifestInfo);
            string manifestFilePath = Path.Combine(buildOption.manifestOutputDir, manifestFileName);
            File.WriteAllText(manifestFilePath, manifestInfo);

            if (builtinModuleInfoDic.Count > 0)
            {
                if (builtinManifest.moduleDic == null || builtinManifest.moduleDic.Count == 0)
                {
                    builtinManifest.moduleDic = builtinModuleInfoDic;
                }
                else
                {
                    foreach (var item in builtinModuleInfoDic)
                    {
                        if (!builtinManifest.moduleDic.ContainsKey(item.Key))
                        {
                            builtinManifest.moduleDic.Add(item.Key, item.Value);
                        }
                        else
                        {
                            builtinManifest.moduleDic[item.Key] = item.Value;
                        }
                    }
                }

                string builtinManifestInfo = JsonUtilityEditor.ToJson(builtinManifest);
                string builtinManifestFilePath = Path.Combine(buildOption.builtinDir, manifestFileName);
                File.WriteAllText(builtinManifestFilePath, builtinManifestInfo);
                AssetDatabase.Refresh();
            }
        }

        public static (AssetModuleManifest, AssetModuleManifest) GetCurrentAssetsManifest()
        {
            AssetBuildOption buildOption = GetDefaultOption();
            if (buildOption == null)
            {
                throw new NullReferenceException("You need to select a build option first.");
            }
            if (!Directory.Exists(buildOption.manifestOutputDir))
                Directory.CreateDirectory(buildOption.manifestOutputDir);
            if (!Directory.Exists(buildOption.builtinDir))
                Directory.CreateDirectory(buildOption.builtinDir);

            string manifestFilePath = Path.Combine(buildOption.manifestOutputDir, manifestFileName);
            string builtinFilePath = Path.Combine(buildOption.builtinDir, manifestFileName);
            AssetModuleManifest allAssetManifest = null;
            AssetModuleManifest builtinManifest = null;
            if (File.Exists(manifestFilePath))
            {
                allAssetManifest = JsonUtilityEditor.ToObject<AssetModuleManifest>(File.ReadAllText(manifestFilePath));
            }
            if (File.Exists(builtinFilePath))
            {
                builtinManifest = JsonUtilityEditor.ToObject<AssetModuleManifest>(File.ReadAllText(builtinFilePath));
            }

            return (allAssetManifest, builtinManifest);
        }

        public static AssetBuildOption GetDefaultOption()
        {
            string optionPath = EditorPrefs.GetString(bundleBuildOptionPrefsKey);
            if (string.IsNullOrEmpty(optionPath))
            {
                return null;
            }
            AssetBuildOption option = AssetDatabase.LoadAssetAtPath<AssetBuildOption>(optionPath);
            return option;
        }

        public static void SetDefaultOption(AssetBuildOption defaultOption)
        {
            string optionPath = AssetDatabase.GetAssetPath(defaultOption);
            EditorPrefs.SetString(bundleBuildOptionPrefsKey, optionPath);
        }

        public static AssetBuildOption CreateNewOption(string tag)
        {
            if (!AssetDatabase.IsValidFolder(buildOptionDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(buildOptionDir));
                AssetDatabase.Refresh();
            }
            string optionPath = $"Assets/Config/EditorConfig/Build/{tag}BuildOption.asset";
            AssetBuildOption option = ScriptableObject.CreateInstance<AssetBuildOption>();
            option.tag = tag;
            option.compressOption = CompressOption.LZ4;
            option.platform = (BuildTargetPlatform)EditorUserBuildSettings.activeBuildTarget;
            AssetDatabase.CreateAsset(option, optionPath);
            option = AssetDatabase.LoadAssetAtPath<AssetBuildOption>(optionPath);

            AssetDatabase.Refresh();
            return option;
        }


        public static List<AssetModuleConfig> GetModuleList()
        {
            List<AssetModuleConfig> modules = new List<AssetModuleConfig>();
            if (!AssetDatabase.IsValidFolder(assetsModuleConfigDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(assetsModuleConfigDir));
                AssetDatabase.Refresh();
            }
            string[] configs = AssetDatabase.FindAssets("", new[] { assetsModuleConfigDir });
            for (var i = 0; i < configs.Length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(configs[i]);
                try
                {
                    AssetModuleConfig module = AssetDatabase.LoadAssetAtPath<AssetModuleConfig>(filePath);
                    module.createTime = new DateTime(module.createTimeTicks);
                    modules.Add(module);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Debug.LogError("Don't put other object under assets module config directory!");
                }
            }
            modules.Sort((module1, module2) => module1.createTimeTicks < module2.createTimeTicks ? -1 : 1);
            return modules;
        }

        public static List<AssetBuildOption> GetOptionList()
        {
            List<AssetBuildOption> options = new List<AssetBuildOption>();
            if (!AssetDatabase.IsValidFolder(buildOptionDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(buildOptionDir));
                AssetDatabase.Refresh();
            }
            string[] configs = AssetDatabase.FindAssets("", new[] { buildOptionDir });
            for (int i = 0; i < configs.Length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(configs[i]);
                try
                {
                    options.Add(AssetDatabase.LoadAssetAtPath<AssetBuildOption>(filePath));
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Debug.LogError("Don't put other object under assets build option directory!");
                }
            }
            return options;
        }

        public static int GetNewModuleID()
        {
            List<AssetModuleConfig> modules = GetModuleList();
            if (modules.Count == 0)
            {
                return 0;
            }

            modules.Sort((module1, module2) => module1.id > module2.id ? -1 : 1);
            return modules[0].id + 1;
        }

        public static bool CreateNewAssetModule(AssetModuleConfig module)
        {
            string fileName = module.moduleName;
            if (string.IsNullOrEmpty(module.moduleName))
            {
                fileName = "NewAssetModule";
            }
            module.createTimeTicks = DateTime.Now.Ticks;
            EditorUtility.SetDirty(module);
            AssetDatabase.SaveAssetIfDirty(module);
            string result = AssetDatabase.RenameAsset(assetsModuleConfigDir + "temp.asset", fileName + ".asset");
            if (!string.IsNullOrEmpty(result))
            {
                Debug.LogError(result);
                return false;
            }
            else
            {
                Debug.Log("successfully create new assets module : " + module.moduleName);
                return true;
            }
        }

        public static bool DeleteAssetModule(AssetModuleConfig module)
        {
            string moduleName = module.moduleName;
            int id = module.id;
            bool confirm = EditorUtility.DisplayDialog("Delete Assets Module", $"are you sure to delete the assets module : {moduleName}?", "Delete", "Cancel");
            if (!confirm)
                return false;
            bool result = AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(module));
            if (result)
            {
                (AssetModuleManifest currentManifest, AssetModuleManifest builtinManifest) = GetCurrentAssetsManifest();
                if (currentManifest != null && currentManifest.moduleDic != null && currentManifest.moduleDic.ContainsKey(id))
                {
                    AssetBuildOption buildOption = GetDefaultOption();
                    currentManifest.moduleDic.Remove(id);
                    string manifestInfo = JsonUtilityEditor.ToJson(currentManifest);
                    string manifestFilePath = Path.Combine(buildOption.manifestOutputDir, manifestFileName);
                    File.WriteAllText(manifestFilePath, manifestInfo);
                }

                if (module.isBuiltin)
                {
                    if (builtinManifest != null && builtinManifest.moduleDic != null && builtinManifest.moduleDic.ContainsKey(id))
                    {
                        AssetBuildOption buildOption = GetDefaultOption();
                        builtinManifest.moduleDic.Remove(id);
                        string manifestInfo = JsonUtilityEditor.ToJson(builtinManifest);
                        string manifestFilePath = Path.Combine(buildOption.builtinDir, manifestFileName);
                        File.WriteAllText(manifestFilePath, manifestInfo);
                    }
                }

                Debug.Log("delete assets module successfully : " + moduleName);
            }
            else
            {
                Debug.LogError("delete assets module error : " + moduleName);
            }
            return result;
        }

        // 模块划分规范约束
        // 1.检查各模块之间的依赖，若某一模块存在对其它模块的依赖，抛出错误
        // 2.检查模块配置
        public static bool CheckAllModulesFormat()
        {
            List<AssetModuleConfig> modules = GetModuleList();
            bool result = true;
            for (int i = 0; i < modules.Count; i++)
            {
                bool passFormatCheck = true;
                AssetModuleConfig module = modules[i];
                if (string.IsNullOrEmpty(module.name))
                {
                    Debug.LogError($"There is a module without name : module ID : {module.id}");
                    passFormatCheck = false;
                }

                if (module.rootFolder == null)
                {
                    Debug.LogError($"There is a module without root reference : module ID : {module.id}, module name : {module.moduleName}");
                    passFormatCheck = false;
                }
                else
                {
                    string rootDir = AssetDatabase.GetAssetPath(module.rootFolder);
                    string[] assets = AssetDatabase.FindAssets("", new string[] { rootDir });
                    if (assets == null || assets.Length == 0)
                    {
                        Debug.LogError($"There is a module without assets : module ID : {module.id}, module name : {module.moduleName}");
                        passFormatCheck = false;
                    }
                }

                if (passFormatCheck)
                {
                    AssetBundleBuild[] builds = InitializeModuleBuild(module);
                    string moduleNamePrefix = module.moduleName.ToLower();
                    for (int j = 0; j < builds.Length; j++)
                    {
                        string[] bundleDependencies = AssetDatabase.GetAssetBundleDependencies(builds[j].assetBundleName, true);
                        for (int k = 0; k < bundleDependencies.Length; k++)
                        {
                            string targetModuleNamePrefix = bundleDependencies[k].Split('_')[0];
                            if (targetModuleNamePrefix != moduleNamePrefix)
                            {
                                Debug.LogError($"There is a module depend to other module : module ID : {module.id}, module name : {module.moduleName}, dependency : {targetModuleNamePrefix}");
                                passFormatCheck = false;
                            }
                        }
                    }
                }

                result = passFormatCheck && result;
            }

            if (result)
            {
                Debug.Log("Asset Modules Check Done, Clean.");
            }

            return result;
        }

        public static void UpgradeAssetModuleGenericVersion()
        {
            List<AssetBuildOption> options = GetOptionList();
            for (int i = 0; i < options.Count; i++)
            {
                options[i].genericVersion++;
                EditorUtility.SetDirty(options[i]);
                AssetDatabase.SaveAssetIfDirty(options[i]);
            }

            BuildAllModules();
        }
    }
}
