using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetPublishView : TabContentView
    {
        private List<AssetArchiveData> archiveDataList;

        public AssetPublishView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override async void OnEnable()
        {
            archiveDataList = await HQAssetBuildLauncher.GetAssetArchivesAsync();
        }

        public override void OnGUI()
        {
            // SceneSet Icon
        }

        public override void OnDisable()
        {
            archiveDataList = null;
        }
    }
}
