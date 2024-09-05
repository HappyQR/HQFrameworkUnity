using System.Collections.Generic;
using System.IO;
using System.Linq;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class SeparateHotfixBuild : HotfixBuild
    {
        public SeparateHotfixBuild(AssetBuildOption buildOption, AppBuildConfig appBuildConfig) : base(buildOption, appBuildConfig)
        {
        }

        protected override void GenerateAssetModuleManifest(Dictionary<int, AssetModuleInfo> moduleDic)
        {
            // Generate all modules manifest
            AssetModuleManifest modulesManifest = GetCurrentManifest();
            if (modulesManifest == null)
            {
                modulesManifest = GenerateBaseManifest();
                modulesManifest.isBuiltinManifest = false;
            }
            if (modulesManifest.moduleDic == null || modulesManifest.moduleDic.Count == 0)
            {
                modulesManifest.moduleDic = moduleDic;
            }
            else
            {
                foreach (var module in moduleDic.Values)
                {
                    if (modulesManifest.moduleDic.ContainsKey(module.id))
                    {
                        modulesManifest.moduleDic[module.id] = module;
                    }
                    else
                    {
                        modulesManifest.moduleDic.Add(module.id, module);
                    }
                }
            }
            modulesManifest.releaseNote = releaseNote;
            string modulesManifestFilePath = Path.Combine(Application.dataPath, buildOption.bundleOutputDir, appBuildConfig.internalVersionCode.ToString(), assetManifestFileName);
            string modulesManifestJsonStr = JsonUtilityEditor.ToJson(modulesManifest);
            Debug.Log($"Build Successfully!\n<color=#00ff00>{modulesManifestJsonStr}</color>");
            File.WriteAllText(modulesManifestFilePath, modulesManifestJsonStr);

            // Generate built-in manifest
            Dictionary<int, AssetModuleInfo> builtinModuleDic = new Dictionary<int, AssetModuleInfo>();
            foreach (var module in moduleDic.Values)
            {
                if (module.isBuiltin)
                {
                    builtinModuleDic.Add(module.id, module);
                }
            }
            if (builtinModuleDic.Count == 0)
            {
                return;
            }
            AssetModuleManifest builtinManifest = GetCurrentBuiltinManifest();
            if (builtinManifest == null)
            {
                builtinManifest = GenerateBaseManifest();
                builtinManifest.isBuiltinManifest = true;
            }
            if (builtinManifest.moduleDic == null || builtinManifest.moduleDic.Count == 0)
            {
                builtinManifest.moduleDic = builtinModuleDic;
            }
            else
            {
                foreach (var module in builtinModuleDic.Values)
                {
                    if (builtinManifest.moduleDic.ContainsKey(module.id))
                    {
                        builtinManifest.moduleDic[module.id] = module;
                    }
                    else
                    {
                        builtinManifest.moduleDic.Add(module.id, module);
                    }
                }
            }
            builtinManifest.releaseNote = releaseNote;
            string builtinManifestFilePath = Path.Combine(Application.streamingAssetsPath, buildOption.builtinDir, assetManifestFileName);
            string builtinManifestJsonStr = JsonUtilityEditor.ToJson(builtinManifest);
            File.WriteAllText(builtinManifestFilePath, builtinManifestJsonStr);

            AssetDatabase.Refresh();
        }

        private AssetModuleManifest GenerateBaseManifest()
        {
            AssetModuleManifest manifest = new AssetModuleManifest();
            manifest.productName = appBuildConfig.productName;
            manifest.productVersion = appBuildConfig.productVersion;
            manifest.runtimePlatform = appBuildConfig.runtimePlatform;
            manifest.resourceVersion = buildOption.resourceVersion;
            manifest.minimalSupportedVersion = buildOption.minimalSupportedVersion;
            return manifest;
        }

        protected override AssetModuleInfo PostProcessAssetModuleBuild(AssetModuleConfig module, AssetBundleManifest manifest)
        {
            string moduleDir = Path.Combine(Application.dataPath, buildOption.bundleOutputDir, 
                                            appBuildConfig.internalVersionCode.ToString(), 
                                            module.moduleName,
                                            module.currentPatchVersion.ToString());
            string moduleBuitinDir = Path.Combine(Application.streamingAssetsPath, buildOption.builtinDir, module.moduleName);
            if (Directory.Exists(moduleDir))
                Directory.Delete(moduleDir, true);
            if (Directory.Exists(moduleBuitinDir))
                Directory.Delete(moduleBuitinDir, true);
            Directory.CreateDirectory(moduleDir);
            if (module.isBuiltin)
                Directory.CreateDirectory(moduleBuitinDir);
            string[] bundles = GetModuleBundles(module, manifest);
            AssetModuleInfo moduleInfo = new AssetModuleInfo();
            moduleInfo.id = module.id;
            moduleInfo.moduleName = module.moduleName;
            moduleInfo.description = module.description;
            moduleInfo.isBuiltin = module.isBuiltin;
            moduleInfo.currentPatchVersion = module.currentPatchVersion;
            moduleInfo.minimalSupportedPatchVersion = module.minimalSupportedPatchVersion;
            moduleInfo.releaseNote = module.releaseNote;
            if (module.autoIncreasePatchVersion)
                module.currentPatchVersion++;
            moduleInfo.bundleDic = new Dictionary<string, AssetBundleInfo>(bundles.Length);
            moduleInfo.assetsDic = new Dictionary<uint, AssetItemInfo>();
            HashSet<int> dependenciesSet = new HashSet<int>();
            for (int i = 0; i < bundles.Length; i++)
            {
                string bundleFilePath = Path.Combine(bundleBuildCacheDir, bundles[i]);
                string destFilePath = Path.Combine(moduleDir, bundles[i]);
                File.Copy(bundleFilePath, destFilePath, true);
                if (module.isBuiltin)
                {
                    string desBuiltinPath = Path.Combine(moduleBuitinDir, bundles[i]);
                    File.Copy(bundleFilePath, desBuiltinPath, true);
                }
                AssetBundleInfo bundleInfo = new AssetBundleInfo();
                bundleInfo.moduleID = module.id;
                bundleInfo.bundleName = bundles[i];
                bundleInfo.dependencies = manifest.GetAllDependencies(bundles[i]);
                bundleInfo.md5 = FileUtilityEditor.GetMD5(bundleFilePath);
                bundleInfo.size = FileUtilityEditor.GetFileSize(bundleFilePath);
                moduleInfo.bundleDic.Add(bundleInfo.bundleName, bundleInfo);
                for (int k = 0; k < bundleInfo.dependencies.Length; k++)
                {
                    AssetModuleConfig dependenceModule = GetBundleModule(bundleInfo.dependencies[k]);
                    if (dependenceModule != null && dependenceModule.id != module.id)
                    {
                        Debug.Log($"WARNING!!!  The module {module.moduleName} has dependency on moudle {dependenceModule.moduleName}!!");
                        dependenciesSet.Add(dependenceModule.id);
                    }
                }
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
            moduleInfo.dependencies = dependenciesSet.ToArray();
            AssetDatabase.Refresh();

            return moduleInfo;
        }

        protected override List<AssetBundleBuild> PreProcessAssetModuleBuild(AssetModuleConfig module)
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
            return builds;
        }

        private AssetModuleManifest GetCurrentManifest()
        {
            string manifestPath = Path.Combine(Application.dataPath, buildOption.bundleOutputDir, appBuildConfig.internalVersionCode.ToString(), assetManifestFileName);
            if (File.Exists(manifestPath))
            {
                string manifestJsonStr = File.ReadAllText(manifestPath);
                AssetModuleManifest manifest = JsonUtilityEditor.ToObject<AssetModuleManifest>(manifestJsonStr);
                return manifest;
            }

            return null;
        }

        private AssetModuleManifest GetCurrentBuiltinManifest()
        {
            string manifestPath = Path.Combine(Application.streamingAssetsPath, buildOption.builtinDir, assetManifestFileName);
            if (File.Exists(manifestPath))
            {
                string manifestJsonStr = File.ReadAllText(manifestPath);
                AssetModuleManifest manifest = JsonUtilityEditor.ToObject<AssetModuleManifest>(manifestJsonStr);
                return manifest;
            }

            return null;
        }
    }
}
