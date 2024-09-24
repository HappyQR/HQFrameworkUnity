using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HQFramework.Resource;
using UnityEditor;

namespace HQFramework.Editor
{
    public class DefaultAssetPostprocessor : IAssetBuildPostprocessor
    {
        private List<AssetModuleConfig> moduleList;

        public AssetModuleBuildResult[] PostprocessModules(AssetCompileResult compileResult)
        {
            List<AssetModuleBuildResult> buildResults = new List<AssetModuleBuildResult>();
            moduleList = new List<AssetModuleConfig>();
            foreach (var moduleConfig in compileResult.moduleBundleDic.Keys)
            {
                moduleList.Add(moduleConfig);
            }

            foreach (var moduleConfig in compileResult.moduleBundleDic.Keys)
            {
                AssetModuleBuildResult moduleBuildResult = new AssetModuleBuildResult();
                moduleBuildResult.moduleID = moduleConfig.id;
                moduleBuildResult.moduleName = moduleConfig.name;
                moduleBuildResult.buildVersionCode = moduleConfig.buildVersionCode;
                moduleBuildResult.devNotes = moduleConfig.devNotes;
                moduleBuildResult.buildTime = DateTime.Now;
                moduleBuildResult.bundlePathMap = new Dictionary<string, AssetBundleInfo>();
                moduleBuildResult.assetsDic = new Dictionary<uint, AssetItemInfo>();
                HashSet<int> dependencySet = new HashSet<int>();

                AssetBundleBuildResult[] bundleBuildResults = compileResult.moduleBundleDic[moduleConfig];
                for (int i = 0; i < bundleBuildResults.Length; i++)
                {
                    AssetBundleInfo bundleInfo = new AssetBundleInfo();
                    bundleInfo.moduleID = moduleConfig.id;
                    bundleInfo.moduleName = moduleConfig.moduleName;
                    bundleInfo.bundleName = bundleBuildResults[i].bundleName;
                    bundleInfo.md5 = bundleBuildResults[i].md5;
                    bundleInfo.size = bundleBuildResults[i].size;
                    bundleInfo.dependencies = bundleBuildResults[i].dependencies;
                    
                    bundleInfo.bundleUrlRelatedToModule = bundleInfo.bundleName;
                    moduleBuildResult.bundlePathMap.Add(bundleBuildResults[i].filePath, bundleInfo);

                    for (int j = 0; j < bundleInfo.dependencies.Length; j++)
                    {
                        AssetModuleConfig dependenceModule = GetBundleModule(bundleInfo.dependencies[j]);
                        if (dependenceModule != moduleConfig)
                        {
                            dependencySet.Add(dependenceModule.id);
                        }
                    }

                    string[] assetsPathArr = AssetDatabase.GetAssetPathsFromAssetBundle(bundleInfo.bundleName);
                    for (int j = 0; j < assetsPathArr.Length; j++)
                    {
                        AssetItemInfo assetItem = new AssetItemInfo();
                        assetItem.assetPath = assetsPathArr[j];
                        assetItem.assetName = Path.GetFileName(assetsPathArr[j]);
                        assetItem.bundleName = bundleInfo.bundleName;
                        assetItem.moduleID = moduleConfig.id;
                        assetItem.crc = Utility.CRC32.ComputeCrc32(assetsPathArr[j]);
                        moduleBuildResult.assetsDic.Add(assetItem.crc, assetItem);
                    }
                }
                moduleBuildResult.dependencies = dependencySet.ToArray();
                buildResults.Add(moduleBuildResult);
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
