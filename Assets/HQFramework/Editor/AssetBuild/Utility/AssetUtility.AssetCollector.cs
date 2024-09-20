using System;
using System.Collections.Generic;
using UnityEditor;

namespace HQFramework.Editor
{
    public static partial class AssetUtility
    {
        public static class AssetCollector
        {
            public static List<KeyValuePair<uint, string>> CollectAssets(string folder)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    throw new InvalidOperationException("Invalid Assets Folder Path.");
                }
                string[] assetPaths = AssetDatabase.FindAssets("", new string[] { folder });
                List<KeyValuePair<uint, string>> assetList = new List<KeyValuePair<uint, string>>(assetPaths.Length);
                for (int i = 0; i < assetPaths.Length; i++)
                {
                    assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
                    if (!AssetDatabase.IsValidFolder(assetPaths[i]))
                    {
                        KeyValuePair<uint, string> asset = new KeyValuePair<uint, string>(Utility.CRC32.ComputeCrc32(assetPaths[i]), assetPaths[i]);
                        assetList.Add(asset);
                    }
                }

                return assetList;
            }
        }
    }
}
