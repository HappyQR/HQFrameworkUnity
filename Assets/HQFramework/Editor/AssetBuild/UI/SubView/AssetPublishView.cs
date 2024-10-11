using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetPublishView : TabContentView
    {
        public AssetPublishView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override void OnGUI()
        {
            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Product Name: ", headerStyle);
            // GUILayout.Label(Application.productName);
            // GUILayout.FlexibleSpace();
            // // GUILayout.Label($"Build Tag : {buildOption.optionTag}", "AssetLabel");
            // GUILayout.EndHorizontal();

            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Current Product Version: ", headerStyle);
            // GUILayout.Label(Application.version);
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();

            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Runtime Platform: ", headerStyle);
            // GUILayout.Label(buildOption.platform.ToString());
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();
            // GUILayout.Space(10);
            // GUILayout.BeginHorizontal();
            

            // GUIContent btnUploadContent = EditorGUIUtility.IconContent("d_RotateTool On");
            // btnUploadContent.text = " Sync With Server";
            // if (GUILayout.Button(btnUploadContent, GUILayout.Height(45), GUILayout.Width((viewRect.width - 30) / 2)))
            // {
            //     EditorApplication.delayCall += AssetsUploader.SyncAssetsWithServer;
            // }
            // GUILayout.EndHorizontal();
        }
    }
}
