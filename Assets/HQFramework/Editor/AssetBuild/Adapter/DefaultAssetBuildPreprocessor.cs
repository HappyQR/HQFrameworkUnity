using System.Collections.Generic;
using UnityEditor;

namespace HQFramework.Editor
{
    public class DefaultAssetBuildPreprocessor : IAssetBuildPreprocessor
    {
        public AssetPreprocessData PreprocessAssetModules(List<AssetModuleConfig> moduleConfigList)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetPreprocessData preprocessData = new AssetPreprocessData();
            for (int i = 0; i < moduleConfigList.Count; i++)
            {
                AssetModuleConfig moduleConfig = moduleConfigList[i];
                for (int j = 0; j < moduleConfig.bundleConfigList.Count; j++)
                {
                    AssetBundleConfig bundleConfig = moduleConfig.bundleConfigList[j];
                    for (int k = 0; k < bundleConfig.assetItemList.Count; k++)
                    {
                        AssetImporter assetImporter = AssetImporter.GetAtPath(bundleConfig.assetItemList[k]);
                        assetImporter.assetBundleName = null;
                        assetImporter.assetBundleName = bundleConfig.bundleName;
                    }
                }
            }

            preprocessData.moduleConfigList = moduleConfigList;
            return preprocessData;
        }
    }
}
