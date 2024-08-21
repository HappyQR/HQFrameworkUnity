using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace HQFramework.Editor
{
    public class UnityEditorUtility
    {
        public static Vector2 GetEditorMainWindowCenter()
        {
            var containerWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ContainerWindow");
            if (containerWindowType == null) throw new InvalidOperationException("ContainerWindow type is null.");

            var windows = Resources.FindObjectsOfTypeAll(containerWindowType);
            if (windows == null || windows.Length == 0) throw new InvalidOperationException("Cannot find any ContainerWindow.");

            Rect mainWindowRect = Rect.zero;
            foreach (var window in windows)
            {
                if (containerWindowType.GetProperty("windowID", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(window, null).ToString() == "UnityEditor.MainView")
                {
                    mainWindowRect = (Rect)containerWindowType.GetProperty("position", BindingFlags.Public | BindingFlags.Instance).GetValue(window, null);
                    break;
                }
            }

            if (mainWindowRect == Rect.zero) throw new InvalidOperationException("MainView window not found.");

            return new Vector2(mainWindowRect.x + mainWindowRect.width / 2, mainWindowRect.y + mainWindowRect.height / 2);
        }

        [MenuItem("HQFramework/Open/PersistentDataPath")]
        public static void OpenPersistentDataPath()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}
