using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HQFramework.Editor
{
    public class DefaultAssetPostprocessor : IAssetBuildPostprocessor
    {
        private List<AssetModuleConfig> moduleList;

        public AssetModuleBuildResult[] PostprocessModules(AssetCompileResult compileResult)
        {
            List<AssetModuleBuildResult> buildResults = new List<AssetModuleBuildResult>();

            foreach (var moduleConfig in compileResult.moduleBundleDic.Keys)
            {
                AssetModuleBuildResult buildInfo = new AssetModuleBuildResult();
                buildInfo.moduleID = moduleConfig.id;
                buildInfo.moduleName = moduleConfig.name;
                buildInfo.buildVersionCode = moduleConfig.buildVersionCode;
                buildInfo.devNotes = moduleConfig.devNotes;
                buildInfo.buildTime = DateTime.Now;

                buildResults.Add(buildInfo);
                moduleConfig.buildVersionCode++;
            }

            return buildResults.ToArray();
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
