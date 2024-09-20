using System;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetBuildConfigView
    {
        private class CreateNewWindow : EditorWindow
        {
            private string tag;
            private static Action<string> confirmCallback;

            public static void Show(Action<string> callback)
            {
                confirmCallback = callback;
                var window = GetWindow<CreateNewWindow>();
                window.titleContent = new GUIContent("Create Build Option");
                window.maxSize = new Vector2(270, 100);
                window.Show();
            }

            void OnDestroy()
            {
                confirmCallback = null;
            }

            private void OnGUI()
            {
                GUILayout.Space(25);

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.Label("Option Tag : ");
                tag = EditorGUILayout.TextField(tag);
                GUILayout.Space(10);
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }
                if (GUILayout.Button("Confirm"))
                {
                    if (string.IsNullOrEmpty(tag))
                    {
                        Debug.LogError("You need to enter a tag!");
                        return;
                    }

                    confirmCallback.Invoke(tag);
                    Close();
                }
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }
    }
}
