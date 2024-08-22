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

        public static readonly string manifestFileName = "AssetModuleManifest.json";

        public static void BuildAllModules()
        {
            List<AssetModuleConfig> modules = AssetModuleManager.GetModuleList();
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
            AssetBundleBuild[] builds = PreprocessModuleBuild(module);
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultOption();
            string bundleOutputDir = Path.Combine(buildOption.bundleOutputDir,
                                                  buildOption.resourceVersion.ToString(),
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

        private static AssetBundleBuild[] PreprocessModuleBuild(AssetModuleConfig module)
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
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultOption();
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
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultOption();
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

        public static void UpgradeAssetModuleGenericVersion()
        {
            List<AssetBuildOption> options = AssetBuildOptionManager.GetOptionList();
            for (int i = 0; i < options.Count; i++)
            {
                options[i].resourceVersion++;
                EditorUtility.SetDirty(options[i]);
                AssetDatabase.SaveAssetIfDirty(options[i]);
            }

            BuildAllModules();
        }
    }
}
