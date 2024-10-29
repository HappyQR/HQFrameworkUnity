using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HQFramework.Runtime;

namespace HQFramework.Editor
{
    [CustomEditor(typeof(GameEntry))]
    public class GameEntryEditor : UnityEditor.Editor
    {
        private SerializedProperty logHelperTypeName;
        
        private string[] logHelperTypeList;
        private int logHelperTypeNameIndex;

        private void OnEnable()
        {
            logHelperTypeName = serializedObject.FindProperty(nameof(logHelperTypeName));
            logHelperTypeList = CollectLogHelperTypes();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Log Helper: ");
            logHelperTypeNameIndex = EditorGUILayout.Popup(logHelperTypeNameIndex, logHelperTypeList);
            logHelperTypeName.stringValue = logHelperTypeList[logHelperTypeNameIndex];
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }

        private string[] CollectLogHelperTypes()
        {
            List<string> typeList = new List<string>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type logHelperInterfaceType = typeof(ILogHelper);
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    if (!types[j].IsAbstract && !types[j].IsInterface && logHelperInterfaceType.IsAssignableFrom(types[j]))
                    {
                        typeList.Add(types[j].FullName);
                    }
                }
            }

            for (int i = 0; i < typeList.Count; i++)
            {
                if (typeList[i] == logHelperTypeName.stringValue)
                {
                    logHelperTypeNameIndex = i;
                }
            }
            
            return typeList.ToArray();
        }
    }
}