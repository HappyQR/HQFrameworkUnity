using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace HQFramework.Editor
{
    public class DefaultAssetBuildPreprocessor : IAssetBuildPreprocessor
    {
        public AssetBundleBuild[] PreprocessModules(List<AssetModuleConfig> moduleConfigList)
        {
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            for (int i = 0; i < moduleConfigList.Count; i++)
            {
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
                        AssetBundleBuild build = new AssetBundleBuild();
                        build.assetBundleName = bundleName;
                        build.assetNames = assets;
                        builds.Add(build);
                    }
                }
            }

            return builds.ToArray();
        }
    }
}