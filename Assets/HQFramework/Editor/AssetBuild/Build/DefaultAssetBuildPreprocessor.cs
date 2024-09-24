using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace HQFramework.Editor
{
    public class DefaultAssetBuildPreprocessor : IAssetBuildPreprocessor
    {
        AssetPreprocessResult IAssetBuildPreprocessor.PreprocessModules(List<AssetModuleConfig> moduleConfigList)
        {
            AssetPreprocessResult preprocessResult = new AssetPreprocessResult();
            for (int i = 0; i < moduleConfigList.Count; i++)
            {
                List<AssetBundleBuildInfo> buildList = new List<AssetBundleBuildInfo>();
                AssetDatabase.RemoveUnusedAssetBundleNames();
                string[] subFolders = AssetDatabase.GetSubFolders(AssetDatabase.GetAssetPath(moduleConfigList[i].rootFolder));
                for (int j = 0; j < subFolders.Length; j++)
                {
                    // Step1: Find all assets under the sub folder and set the bundle name
                    // Step2: Collect the AssetBundleBuild objects
                    string dirName = Path.GetFileName(subFolders[j]);
                    string bundleName = $"{moduleConfigList[i].moduleName}_{dirName}.bundle".ToLower();

                    string[] assets = AssetDatabase.FindAssets("", new[] { subFolders[j] });
                    for (int k = 0; k < assets.Length; k++)
                    {
                        string filePath = AssetDatabase.GUIDToAssetPath(assets[k]);
                        if (AssetDatabase.IsValidFolder(filePath))
                        {
                            continue;
                        }
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
                        AssetBundleBuildInfo build = new AssetBundleBuildInfo();
                        build.bundleName = bundleName;
                        build.bundleAssets = assets;
                        buildList.Add(build);
                    }
                }
                if (buildList.Count > 0)
                {
                    preprocessResult.moduleBundleBuildsDic.Add(moduleConfigList[i], buildList);
                }
            }

            return preprocessResult;
        }
    }
}
