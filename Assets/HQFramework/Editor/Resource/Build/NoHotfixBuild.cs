using System.Collections.Generic;
using System.IO;
using System.Linq;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class NoHotfixBuild : HotfixBuild
    {
        public NoHotfixBuild(AssetBuildOption buildOption, AppBuildConfig appBuildConfig) : base(buildOption, appBuildConfig)
        {
        }

        protected override void OnBuildStart()
        {
            string builtinDir = Path.Combine(Application.streamingAssetsPath, buildOption.builtinDir);
            if (Directory.Exists(builtinDir))
            {
                Directory.Delete(builtinDir, true);
                Directory.CreateDirectory(builtinDir);
            }
        }

        protected override void OnBuildSuccess()
        {
            if (buildOption.autoIncreaseResourceVersion)
            {
                buildOption.resourceVersion++;
            }
        }

        protected override void GenerateAssetModuleManifest(Dictionary<int, AssetModuleInfo> moduleDic)
        {
            AssetModuleManifest manifest = new AssetModuleManifest();
            manifest.productName = appBuildConfig.productName;
            manifest.productVersion = appBuildConfig.productVersion;
            manifest.runtimePlatform = appBuildConfig.runtimePlatform;
            manifest.resourceVersion = buildOption.resourceVersion;
            manifest.moduleDic = moduleDic;
            manifest.isBuiltinManifest = true;
            string builtinManifestJsonStr = JsonUtilityEditor.ToJson(manifest);
            string builtinManifestFilePath = Path.Combine(Application.streamingAssetsPath, buildOption.builtinDir, assetManifestFileName);
            File.WriteAllText(builtinManifestFilePath, builtinManifestJsonStr);
            Debug.Log($"Build Successfully!\n<color=#00ff00>{builtinManifestJsonStr}</color>");

            AssetDatabase.Refresh();
        }

        protected override AssetModuleInfo PostProcessAssetModuleBuild(AssetModuleConfig module, AssetBundleManifest manifest)
        {
            string moduleBuitinDir = Path.Combine(Application.streamingAssetsPath, buildOption.builtinDir, module.moduleName);
            Directory.CreateDirectory(moduleBuitinDir);
            string[] bundles = GetModuleBundles(module, manifest);
            AssetModuleInfo moduleInfo = new AssetModuleInfo();
            moduleInfo.id = module.id;
            moduleInfo.moduleName = module.moduleName;
            moduleInfo.description = module.description;
            moduleInfo.isBuiltin = true;
            moduleInfo.bundleDic = new Dictionary<string, AssetBundleInfo>(bundles.Length);
            moduleInfo.assetsDic = new Dictionary<uint, AssetItemInfo>();
            HashSet<int> dependenciesSet = new HashSet<int>();
            for (int i = 0; i < bundles.Length; i++)
            {
                string bundleFilePath = Path.Combine(bundleBuildCacheDir, bundles[i]);
                string desBuiltinPath = Path.Combine(moduleBuitinDir, bundles[i]);
                File.Copy(bundleFilePath, desBuiltinPath, true);
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
    }
}
