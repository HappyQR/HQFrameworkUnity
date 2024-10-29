using System.Collections.Generic;
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

        private ProcedureComponent targetComponent;

        private int entryIndex;

        private void OnEnable()
        {
            targetComponent = target as ProcedureComponent;
            gameProcedures = serializedObject.FindProperty(nameof(gameProcedures));
            entryProcedure = serializedObject.FindProperty(nameof(entryProcedure));

            for (int i = 0; i < gameProcedures.arraySize; i++)
            {
                if (gameProcedures.GetArrayElementAtIndex(i).stringValue == entryProcedure.stringValue)
                {
                    entryIndex = i;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            GUIStyle headerStyle = "AM HeaderStyle";

            EditorGUILayout.PropertyField(gameProcedures);

            EditorGUILayout.Separator();

            if (targetComponent.gameProcedures != null && targetComponent.gameProcedures.Length > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Entry Procedure: ", headerStyle);
                entryIndex = EditorGUILayout.Popup(entryIndex, targetComponent.gameProcedures, GUILayout.ExpandWidth(true));
                entryProcedure.stringValue = targetComponent.gameProcedures[entryIndex];
                GUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
