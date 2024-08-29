using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public abstract class HotfixBuild
    {
        List<AssetModuleConfig> modules;
        protected AssetBuildOption buildOption;
        protected AppBuildConfig appBuildConfig;
        protected string bundleBuildCacheDir;

        public HotfixBuild(AssetBuildOption buildOption, AppBuildConfig appBuildConfig)
        {
            this.buildOption = buildOption;
            this.appBuildConfig = appBuildConfig;
            bundleBuildCacheDir = Path.Combine(Application.dataPath, buildOption.bundleOutputDir, "BuildCache");
            if (!Directory.Exists(bundleBuildCacheDir))
            {
                Directory.CreateDirectory(bundleBuildCacheDir);
            }
        }

        protected abstract List<AssetBundleBuild> PreProcessAssetModuleBuild(AssetModuleConfig module);

        protected abstract AssetModuleInfo PostProcessAssetModuleBuild(AssetModuleConfig module, AssetBundleManifest manifest);

        public virtual void BuildAssetMoudles(List<AssetModuleConfig> modules)
        {
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
            AssetBuildUtility.GenerateAssetManifest(moduleDic);
            OnBuildSuccess();
        }

        public virtual void ClearBuilds()
        {
            string bundleOutputDir = Path.Combine(Application.dataPath, buildOption.bundleOutputDir);
            string bundleBuiltinDir = Path.Combine(Application.persistentDataPath, buildOption.builtinDir);

            Directory.Delete(bundleOutputDir, true);
            Directory.Delete(bundleBuiltinDir, true);
        }

        protected virtual void OnBuildSuccess()
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
