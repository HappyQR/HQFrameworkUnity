using System.Collections.Generic;
using System.IO;
using System.Linq;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public abstract class AssetBuilder
    {
        public static readonly string assetManifestFileName = "AssetModuleManifest.json";

        private List<AssetModuleConfig> modules;
        protected AssetBuildOption buildOption;
        protected string releaseNote;
        protected string bundleBuildCacheDir;
        protected string manifestOutputDir;
        protected string assetOutputDir;
        protected string assetBuiltinDir;

        public AssetBuilder(AssetBuildOption buildOption, AppBuildConfig appBuildConfig)
        {
            this.buildOption = buildOption;
            assetOutputDir = Path.Combine(Application.dataPath, buildOption.bundleOutputDir);
            assetBuiltinDir = Path.Combine(Application.streamingAssetsPath, buildOption.builtinDir);
            bundleBuildCacheDir = Path.Combine(assetOutputDir, "BuildCache");
            manifestOutputDir = Path.Combine(assetOutputDir, assetManifestFileName);

            if (!Directory.Exists(bundleBuildCacheDir))
            {
                Directory.CreateDirectory(bundleBuildCacheDir);
            }
        }

        public virtual void BuildAssetMoudles(List<AssetModuleConfig> modules, string releaseNote)
        {
            this.releaseNote = releaseNote;
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            for(int i = 0; i < modules.Count; i++)
            {
                List<AssetBundleBuild> bundleBuilds = PreProcessAssetModuleBuild(modules[i]);
                builds.AddRange(bundleBuilds);
            }
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(bundleBuildCacheDir, 
                                                                           builds.ToArray(), 
                                                                           (BuildAssetBundleOptions)buildOption.compressOption,
                                                                           (BuildTarget)buildOption.platform);
            if (manifest == null)
            {
                Debug.LogError("Build Failed");
                return;
            }

            Dictionary<int, AssetModuleInfo> moduleDic = new Dictionary<int, AssetModuleInfo>();
            for (int i = 0; i < modules.Count; i++)
            {
                AssetModuleInfo info = PostProcessAssetModuleBuild(modules[i], manifest);
                moduleDic.Add(info.id, info);
            }
            GenerateAssetModuleManifest(moduleDic);
            AssetDatabase.Refresh();
        }

        protected virtual List<AssetBundleBuild> PreProcessAssetModuleBuild(AssetModuleConfig module)
        {
            module.currentPatchVersion = module.nextPatchVersion;
            if (module.autoIncreasePatchVersion)
            {
                module.nextPatchVersion++;
            }
            EditorUtility.SetDirty(module);
            AssetDatabase.SaveAssetIfDirty(module);

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
                        importer.assetBundleName = null;
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

        protected virtual AssetModuleInfo PostProcessAssetModuleBuild(AssetModuleConfig moduleConfig, AssetBundleManifest manifest)
        {
            string moduleOutputDir = Path.Combine(assetOutputDir, buildOption.resourceVersion.ToString(), moduleConfig.moduleName, moduleConfig.currentPatchVersion.ToString());
            string moduleBuiltinDir = Path.Combine(assetBuiltinDir, moduleConfig.moduleName);
            AssetModuleInfo moduleInfo = new AssetModuleInfo();
            moduleInfo.id = moduleConfig.id;
            moduleInfo.moduleName = moduleConfig.moduleName;
            moduleInfo.description = moduleConfig.description;
            moduleInfo.currentPatchVersion = moduleConfig.currentPatchVersion;
            moduleInfo.minimalSupportedPatchVersion = moduleConfig.minimalSupportedPatchVersion;
            moduleInfo.isBuiltin = moduleConfig.isBuiltin;
            moduleInfo.moduleUrlRelatedToHotfixUrlRoot = $"{buildOption.resourceVersion}/{moduleInfo.moduleName}/{moduleInfo.currentPatchVersion}";
            moduleInfo.releaseNote = moduleConfig.releaseNote;
            moduleInfo.bundleDic = new Dictionary<string, AssetBundleInfo>();
            moduleInfo.assetsDic = new Dictionary<uint, AssetItemInfo>();
            HashSet<int> dependencySet = new HashSet<int>();
            string[] bundles = GetModuleBundles(moduleConfig, manifest);
            for (int i = 0; i < bundles.Length; i++)
            {
                string bundleOutputFilePath = Path.Combine(bundleBuildCacheDir, bundles[i]);

                AssetBundleInfo bundleInfo = new AssetBundleInfo();
                bundleInfo.moduleID = moduleConfig.id;
                bundleInfo.moduleName = moduleConfig.moduleName;
                bundleInfo.bundleName = bundles[i];
                bundleInfo.md5 = Utility.Hash.ComputeHash(bundleOutputFilePath);
                bundleInfo.size = FileUtilityEditor.GetFileSize(bundleOutputFilePath);
                bundleInfo.bundleUrlRelatedToModule = bundles[i];
                bundleInfo.dependencies = manifest.GetAllDependencies(bundles[i]);
                moduleInfo.bundleDic.Add(bundleInfo.bundleName, bundleInfo);
                for (int k = 0; k < bundleInfo.dependencies.Length; k++)
                {
                    AssetModuleConfig dependenceModule = GetBundleModule(bundleInfo.dependencies[k]);
                    if (dependenceModule != null && dependenceModule.id != moduleConfig.id)
                    {
                        dependencySet.Add(dependenceModule.id);
                    }
                }
                string[] assetsPathArr = AssetDatabase.GetAssetPathsFromAssetBundle(bundles[i]);
                for (int j = 0; j < assetsPathArr.Length; j++)
                {
                    AssetItemInfo assetItem = new AssetItemInfo();
                    assetItem.assetPath = assetsPathArr[j];
                    assetItem.assetName = Path.GetFileName(assetsPathArr[j]);
                    assetItem.bundleName = bundles[i];
                    assetItem.moduleID = moduleConfig.id;
                    assetItem.crc = Utility.CRC32.ComputeCrc32(assetsPathArr[j]);
                    moduleInfo.assetsDic.Add(assetItem.crc, assetItem);
                }
            }
            moduleInfo.dependencies = dependencySet.ToArray();
            return moduleInfo;
        }

        protected virtual void GenerateAssetModuleManifest(Dictionary<int, AssetModuleInfo> moduleDic)
        {

        }

        protected string[] GetModuleBundles(AssetModuleConfig module, AssetBundleManifest manifest)
        {
            string[] allBundles = manifest.GetAllAssetBundles();
            List<string> bundles = new List<string>();
            string modulePrefix = module.moduleName.ToLower();
            for (int i = 0; i < allBundles.Length; i++)
            {
                if (allBundles[i].StartsWith(modulePrefix))
                {
                    bundles.Add(allBundles[i]);
                }
            }
            return bundles.ToArray();
        }

        protected AssetModuleConfig GetBundleModule(string bundleName)
        {
            if (modules == null)
            {
                modules = AssetModuleConfigManager.GetModuleList();
            }
            string modulePrefix = bundleName.Substring(0, bundleName.LastIndexOf('_'));
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].moduleName.ToLower() == modulePrefix)
                {
                    return modules[i];
                }
            }
            return null;
        }
    }
}
