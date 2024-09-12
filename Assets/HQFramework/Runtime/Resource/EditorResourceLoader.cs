#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;

using UnityObject = UnityEngine.Object;

namespace HQFramework.Runtime
{
    public class EditorResourceLoader
    {
        private static readonly string assetsRootDir = "Assets/GameAssets/";


        public void LoadAsset(AssetItemInfo assetInfo, Type assetType, Action<object> callback)
        {
            UnityObject target = AssetDatabase.LoadAssetAtPath(assetInfo.assetPath, assetType);
            target = UnityObject.Instantiate(target);
            callback?.Invoke(target);
        }

        public void LoadAsset<T>(AssetItemInfo assetInfo, Action<T> callback) where T : class
        {
            UnityObject target = AssetDatabase.LoadAssetAtPath(assetInfo.assetPath, typeof(T));
            target = UnityObject.Instantiate(target);
            callback?.Invoke(target as T);
        }

        public Dictionary<uint, AssetItemInfo> LoadAssetsMap()
        {
            List<AssetItemInfo> list = GetAllAssetItems(assetsRootDir);
            Dictionary<uint, AssetItemInfo> assetsDic = new Dictionary<uint, AssetItemInfo>();
            foreach (AssetItemInfo item in list)
            {
                assetsDic.Add(item.crc, item);
            }
            return assetsDic;
        }

        private List<AssetItemInfo> GetAllAssetItems(string path)
        {
            List<AssetItemInfo> assetsList = new List<AssetItemInfo>();

            return assetsList;
        }

        private List<AssetItemInfo> GetAssetsFromAssetBundle(string bundleName)
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

#endif