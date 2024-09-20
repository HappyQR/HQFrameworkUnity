using System.Collections.Generic;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetPostprocessor : IAssetBuildPostprocessor
    {
        private List<AssetModuleConfig> moduleList;

        public void PostprocessModules(List<AssetModuleConfig> moduleConfigList, AssetBundleManifest buildManifest)
        {
            this.moduleList = moduleConfigList;
        }

        protected string[] GetModuleBundles(AssetModuleConfig module, AssetBundleManifest buildManifest)
        {
            string[] allBundles = buildManifest.GetAllAssetBundles();
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
            string modulePrefix = bundleName.Substring(0, bundleName.LastIndexOf('_'));
            for (int i = 0; i < moduleList.Count; i++)
            {
                if (moduleList[i].moduleName.ToLower() == modulePrefix)
                {
                    return moduleList[i];
                }
            }
            return null;
        }
    }
}
