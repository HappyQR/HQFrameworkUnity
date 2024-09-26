using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HQFramework.Editor
{
    public partial class AssetArchiveView
    {
        private class ArchiveNotesWindow : EditorWindow
        {
            private List<AssetModuleBuildResult> moduleBuildResultList;
            private string archiveNotes;

            public static void ShowWindow(List<AssetModuleBuildResult> moduleBuildResultList)
            {
                var window = GetWindow<ArchiveNotesWindow>();
                window.moduleBuildResultList = moduleBuildResultList;
                window.minSize = window.maxSize = new Vector2(480, 320);
                window.titleContent = new GUIContent("Asset Archive");
                window.Show();
            }

            private void OnGUI()
            {
                GUILayout.BeginArea(new Rect(10, 10, position.width - 20, position.height - 20));
                GUIStyle headerStyle = "AM HeaderStyle";

                GUILayout.Space(10);
                GUILayout.Label("Archive Notes: ", headerStyle);
                GUILayout.Space(20);
                archiveNotes = EditorGUILayout.TextArea(archiveNotes, GUILayout.Height(200));
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
                        AssetArchiver.PackArchive(moduleBuildResultList);
                        Close();
                    };
                }
                GUILayout.EndHorizontal();

                GUILayout.EndArea();
            }

            private void OnDestroy()
            {
                moduleBuildResultList = null;
                archiveNotes = null;
            }
        }
    }
}
