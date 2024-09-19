using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetTableView : TabContentView
    {
        private string searchPattern = "";
        private Object rootFolder = null;
        private Object lastRootFolder = null;
        private List<AssetItemInfo> assetList;
        private Vector2 scrollPos;

        public AssetTableView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override void OnGUI()
        {
            GUIStyle headerStyle = "AM HeaderStyle";

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Asset Folder: ", headerStyle);
            rootFolder = EditorGUILayout.ObjectField(rootFolder, typeof(DefaultAsset), false);
            if (lastRootFolder != rootFolder)
            {
                ReCollectAssets();
                lastRootFolder = rootFolder;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search: ", headerStyle);
            searchPattern = GUILayout.TextField(searchPattern, GUILayout.Height(20), GUILayout.Width(viewRect.width - 70));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("CRC", headerStyle, GUILayout.Width(100));
            GUILayout.Label("Path", headerStyle, GUILayout.Width(viewRect.width - 120));
            GUILayout.EndHorizontal();

            if (assetList == null)
            {
                return;
            }

            GUILayout.Space(10);

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            scrollPos.x = 0;

            for (int i = 0; i < assetList.Count; i++)
            {
                if (assetList[i].assetPath.ToLower().Contains(searchPattern.ToLower()))
                {
                    GUILayout.BeginHorizontal();

                    EditorGUILayout.SelectableLabel(assetList[i].crc.ToString(), headerStyle, GUILayout.Width(100));
                    EditorGUILayout.SelectableLabel(assetList[i].assetPath, headerStyle, GUILayout.Width(viewRect.width - 120));

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.Space(10);
        }

        private void ReCollectAssets()
        {
            string path = AssetDatabase.GetAssetPath(rootFolder);
            string[] assetPaths = AssetDatabase.FindAssets("", new string[] { path });
            assetList = new List<AssetItemInfo>(assetPaths.Length);
            for (int i = 0; i < assetPaths.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
                if (!AssetDatabase.IsValidFolder(assetPaths[i]))
                {
                    AssetItemInfo asset = new AssetItemInfo();
                    asset.assetPath = assetPaths[i];
                    asset.crc = Utility.CRC32.ComputeCrc32(assetPaths[i]);
                    assetList.Add(asset);
                }
            }
        }
    }
}
