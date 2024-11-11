using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HQFramework.Resource;
using UnityEditor;

namespace HQFramework.Editor
{
    public class DefaultAssetBuildPostprocessor : IAssetBuildPostprocessor
    {
        private Dictionary<string, AssetModuleConfig> bundleModuleMap;

        public AssetPostprocessData PostprocessAssetModules(AssetCompileData compileData)
        {
            bundleModuleMap = new Dictionary<string, AssetModuleConfig>();
            foreach (KeyValuePair<AssetModuleConfig, List<AssetBundleCompileInfo>> pair in compileData.dataDic)
            {
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    bundleModuleMap.Add(pair.Value[i].bundleName, pair.Key);
                }
            }
            AssetPostprocessData postprocessData = new AssetPostprocessData();
            foreach (KeyValuePair<AssetModuleConfig, List<AssetBundleCompileInfo>> item in compileData.dataDic)
            {
                AssetModuleCompileInfo moduleCompileInfo = new AssetModuleCompileInfo();
                moduleCompileInfo.moduleID = item.Key.id;
                moduleCompileInfo.moduleName = item.Key.moduleName;
                moduleCompileInfo.buildVersionCode = item.Key.buildVersionCode;
                moduleCompileInfo.isBuiltin = item.Key.isBuiltin;
                moduleCompileInfo.devNotes = item.Key.devNotes;
                moduleCompileInfo.buildTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                moduleCompileInfo.bundleList = item.Value;
                moduleCompileInfo.assetsDic = new Dictionary<uint, HQAssetItemConfig>();
                for (int i = 0; i < item.Key.bundleConfigList.Count; i++)
                {
                    AssetBundleConfig bundleConfig = item.Key.bundleConfigList[i];
                    for (int j = 0; j < bundleConfig.assetItemList.Count; j++)
                    {
                        string assetPath = bundleConfig.assetItemList[j];
                        HQAssetItemConfig assetItem = new HQAssetItemConfig();
                        assetItem.crc = Utility.CRC32.ComputeCrc32(assetPath);
                        assetItem.assetPath = assetPath;
                        assetItem.assetName = Path.GetFileName(assetPath);
                        assetItem.bundleName = bundleConfig.bundleName;
                        assetItem.moduleID = item.Key.id;
                        List<uint> dependencyCrcList = new List<uint>();
                        string[] assetDependencies = AssetDatabase.GetDependencies(assetPath, false);
                        for (int k = 0; k < assetDependencies.Length; k++)
                        {
                            if (assetDependencies[k] == assetPath)
                                continue;
                            string dependenceBundleName = AssetDatabase.GetImplicitAssetBundleName(assetDependencies[k]);
                            if (!string.IsNullOrEmpty(dependenceBundleName) && bundleModuleMap.ContainsKey(dependenceBundleName))
                            {
                                dependencyCrcList.Add(Utility.CRC32.ComputeCrc32(assetDependencies[k]));
                            }
                        }
                        assetItem.dependencies = dependencyCrcList.ToArray();
                        moduleCompileInfo.assetsDic.Add(assetItem.crc, assetItem);
                    }
                }

                HashSet<int> dependencySet = new HashSet<int>();
                for (int i = 0; i < item.Value.Count; i++)
                {
                    AssetBundleCompileInfo bundleCompileInfo = item.Value[i];
                    for (int j = 0; j < bundleCompileInfo.dependencies.Length; j++)
                    {
                        AssetModuleConfig dependenceModule = GetBundleModule(bundleCompileInfo.dependencies[j]);
                        if (dependenceModule.id != item.Key.id)
                        {
                            dependencySet.Add(dependenceModule.id);
                        }
                    }
                }
                moduleCompileInfo.dependencies = dependencySet.ToArray();

                postprocessData.dataList.Add(moduleCompileInfo);
            }
            return postprocessData;
        }

        protected AssetModuleConfig GetBundleModule(string bundleName)
        {
            return bundleModuleMap[bundleName];
        }
    }
}
