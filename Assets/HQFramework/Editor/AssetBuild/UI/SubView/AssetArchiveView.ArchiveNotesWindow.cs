using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HQFramework.Editor
{
    public partial class AssetArchiveView
    {
        private class ArchiveNotesWindow : EditorWindow
        {
            private List<AssetModuleCompileInfo> moduleCompileList;
            private string archiveTag;
            private string archiveNotes;

            public static void ShowWindow(List<AssetModuleCompileInfo> moduleCompileList)
            {
                var window = GetWindow<ArchiveNotesWindow>();
                window.moduleCompileList = moduleCompileList;
                window.minSize = window.maxSize = new Vector2(480, 320);
                window.titleContent = new GUIContent("Asset Archive");
                window.Show();
            }

            private void OnGUI()
            {
                GUILayout.BeginArea(new Rect(10, 10, position.width - 20, position.height - 20));
                GUIStyle headerStyle = "AM HeaderStyle";

                GUILayout.Label("Archive Tag : ", headerStyle);
                GUILayout.Space(5);
                archiveTag = EditorGUILayout.TextField(archiveTag);

                GUILayout.Space(10);
                GUILayout.Label("Archive Notes: ", headerStyle);
                GUILayout.Space(5);
                archiveNotes = EditorGUILayout.TextArea(archiveNotes, GUILayout.Height(180));
                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Cancel", GUILayout.Height(30)))
                {
                    Close();
                }

                if (GUILayout.Button("Archive", GUILayout.Height(30)))
                {
                    EditorApplication.delayCall += () => 
                    {
                        HQAssetBuildLauncher.ArchiveAssetModules(moduleCompileList, archiveTag, archiveNotes);
                        Close();
                    };
                }
                GUILayout.EndHorizontal();

                GUILayout.EndArea();
            }
        }
    }
}
