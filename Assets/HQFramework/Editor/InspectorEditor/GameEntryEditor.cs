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
        private SerializedProperty gameProcedures;
        private SerializedProperty entryProcedure;

        private string[] logHelperTypeList;
        private int logHelperTypeNameIndex;
        private int procedureIndex;

        private void OnEnable()
        {
            logHelperTypeName = serializedObject.FindProperty(nameof(logHelperTypeName));
            gameProcedures = serializedObject.FindProperty(nameof(gameProcedures));
            entryProcedure = serializedObject.FindProperty(nameof(entryProcedure));

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
            GUILayout.BeginHorizontal();
            GUILayout.Label("Entry Procedure:");
            procedureIndex = EditorGUILayout.Popup(procedureIndex, (target as GameEntry).gameProcedures);
            if ((target as GameEntry).gameProcedures.Length > 0)
            {
                entryProcedure.stringValue = (target as GameEntry).gameProcedures[procedureIndex];
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(gameProcedures);

            serializedObject.ApplyModifiedProperties();
        }

        private string[] CollectLogHelperTypes()
        {
            List<string> typeList = new List<string>();
            Assembly assembly = Assembly.GetAssembly(typeof(GameEntry));
            Type[] types = assembly.GetTypes();
            Type logHelperInterfaceType = typeof(ILogHelper);
            for (int i = 0; i < types.Length; i++)
            {
                if (logHelperInterfaceType.IsAssignableFrom(types[i]))
                {
                    typeList.Add(types[i].FullName);
                }
            }
            return typeList.ToArray();
        }
    }
}