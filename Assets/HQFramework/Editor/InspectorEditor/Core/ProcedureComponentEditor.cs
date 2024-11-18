using System;
using System.Collections.Generic;
using HQFramework.Procedure;
using HQFramework.Runtime;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    [CustomEditor(typeof(ProcedureComponent))]
    public class ProcedureComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty gameProcedures;
        private SerializedProperty entryProcedure;
        private SerializedProperty procedureScripts;

        private ProcedureComponent targetComponent;
        private Type procedureBaseType;

        private List<string> procedureList;

        private int entryIndex;

        private void OnEnable()
        {
            targetComponent = target as ProcedureComponent;
            procedureBaseType = typeof(ProcedureBase);
            gameProcedures = serializedObject.FindProperty(nameof(gameProcedures));
            entryProcedure = serializedObject.FindProperty(nameof(entryProcedure));
            procedureScripts = serializedObject.FindProperty(nameof(procedureScripts));
            
            for (int i = 0; i < gameProcedures.arraySize; i++)
            {
                if (gameProcedures.GetArrayElementAtIndex(i).stringValue == entryProcedure.stringValue)
                {
                    entryIndex = i;
                }
            }

            procedureList = new List<string>();
        }

        public override void OnInspectorGUI()
        {
            GUIStyle headerStyle = "AM HeaderStyle";

            EditorGUILayout.PropertyField(procedureScripts);
            procedureList.Clear();
            gameProcedures.ClearArray();
            for (int i = 0; i < procedureScripts.arraySize; i++)
            {
                MonoScript monoScript = procedureScripts.GetArrayElementAtIndex(i).objectReferenceValue as MonoScript;
                Type scriptType = monoScript.GetClass();
                if (!procedureBaseType.IsAssignableFrom(scriptType) || scriptType.IsInterface || scriptType.IsAbstract)
                {
                    Debug.LogError($"Invalid Type Of Procedure : {scriptType.FullName}");
                }
                else
                {
                    procedureList.Add(scriptType.FullName);
                    gameProcedures.InsertArrayElementAtIndex(procedureList.Count - 1);
                    gameProcedures.GetArrayElementAtIndex(procedureList.Count - 1).stringValue = scriptType.FullName;
                }
            }
            string[] procedures = procedureList.ToArray();

            EditorGUILayout.Separator();

            if (gameProcedures.arraySize > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Entry Procedure: ", headerStyle);
                entryIndex = EditorGUILayout.Popup(entryIndex, procedures, GUILayout.ExpandWidth(true));
                entryProcedure.stringValue = gameProcedures.GetArrayElementAtIndex(entryIndex).stringValue;
                GUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
