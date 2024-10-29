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
        private SerializedProperty jsonHelperTypeName;
        
        private string[] logHelperTypeList;
        private string[] jsonHelperTypeList;
        private int logHelperTypeIndex;
        private int jsonHelperTypeIndex;

        private void OnEnable()
        {
            logHelperTypeName = serializedObject.FindProperty(nameof(logHelperTypeName));
            jsonHelperTypeName = serializedObject.FindProperty(nameof(jsonHelperTypeName));
            CollectTypeList();
        }

        public override void OnInspectorGUI()
        {
            GUIStyle headerStyle = "AM HeaderStyle";

            GUILayout.BeginHorizontal();
            GUILayout.Label("Log Helper: ", headerStyle);
            logHelperTypeIndex = EditorGUILayout.Popup(logHelperTypeIndex, logHelperTypeList, GUILayout.ExpandWidth(true));
            logHelperTypeName.stringValue = logHelperTypeList[logHelperTypeIndex];
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Json Helper: ", headerStyle);
            jsonHelperTypeIndex = EditorGUILayout.Popup(jsonHelperTypeIndex, jsonHelperTypeList, GUILayout.ExpandWidth(true));
            jsonHelperTypeName.stringValue = jsonHelperTypeList[jsonHelperTypeIndex];
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void CollectTypeList()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<string> logHelperTypes = new List<string>();
            List<string> jsonHelperTypes = new List<string>();

            Type logHelperInterfaceType = typeof(ILogHelper);
            Type jsonHelperInterfaceType = typeof(IJsonHelper);

            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    if (!types[j].IsAbstract && !types[j].IsInterface)
                    {
                        if (logHelperInterfaceType.IsAssignableFrom(types[j]))
                        {
                            logHelperTypes.Add(types[j].FullName);
                        }
                        else if (jsonHelperInterfaceType.IsAssignableFrom(types[j]))
                        {
                            jsonHelperTypes.Add(types[j].FullName);    
                        }
                    }
                }
            }

            for (int i = 0; i < logHelperTypes.Count; i++)
            {
                if (logHelperTypes[i] == logHelperTypeName.stringValue)
                {
                    logHelperTypeIndex = i;
                }
            }

            for (int i = 0; i < jsonHelperTypes.Count; i++)
            {
                if (jsonHelperTypes[i] == jsonHelperTypeName.stringValue)
                {
                    jsonHelperTypeIndex = i;
                }
            }
            
            logHelperTypeList = logHelperTypes.ToArray();
            jsonHelperTypeList = jsonHelperTypes.ToArray();
        }
    }
}