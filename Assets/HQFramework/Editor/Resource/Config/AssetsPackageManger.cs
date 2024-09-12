using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;

namespace HQFramework.Editor
{
    public class AssetsPackageManger
    {
        public static List<AssetItemInfo> GetAllAssetItems(string path)
        {
            List<AssetItemInfo> assetsList = new List<AssetItemInfo>();

            return assetsList;
        }

        public static List<AssetItemInfo> GetAssetsFromAssetBundle(string bundleName)
        {
            List<AssetItemInfo> assetsList = new List<AssetItemInfo>();
            string[] assetsPathArr = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            for (int j = 0; j < assetsPathArr.Length; j++)
            {
                AssetItemInfo assetItem = new AssetItemInfo();
                assetItem.assetPath = assetsPathArr[j];
                assetItem.assetName = Path.GetFileName(assetsPathArr[j]);
                assetItem.bundleName = bundleName;
                assetItem.crc = Utility.CRC32.ComputeCrc32(assetsPathArr[j]);
                assetsList.Add(assetItem);
            }
            return assetsList;
        }
    }
}
