using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    public class DefaultAssetBuildPostprocessor : IAssetBuildPostprocessor
    {
        private List<AssetModuleConfig> moduleList;

        public AssetPostprocessData PostprocessAssetModules(AssetCompileData compileData)
        {
            moduleList = new List<AssetModuleConfig>();
            foreach (AssetModuleConfig item in compileData.dataDic.Keys)
            {
                moduleList.Add(item);
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
