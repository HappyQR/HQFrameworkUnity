using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HQFramework.Runtime;
using System.Reflection;
using System;
using HQFramework.UI;

namespace HQFramework.Editor
{
    [CustomEditor(typeof(UIComponent))]
    public class UIComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty helperTypeName;

        private string[] helperTypeList;
        private int helperTypeIndex;

        private void OnEnable()
        {
            helperTypeName = serializedObject.FindProperty(nameof(helperTypeName));

            helperTypeList = CollectUIHelperTypes();
        }

        public override void OnInspectorGUI()
        {
            GUIStyle headerStyle = "AM HeaderStyle";
            GUILayout.BeginHorizontal();
            GUILayout.Label("UI Helper: ", headerStyle);
            helperTypeIndex = EditorGUILayout.Popup(helperTypeIndex, helperTypeList, GUILayout.ExpandWidth(true));
            helperTypeName.stringValue = helperTypeList[helperTypeIndex];
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private string[] CollectUIHelperTypes()
        {
            List<string> typeList = new List<string>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type helperInterfaceType = typeof(IUIHelper);
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    if (!types[j].IsAbstract && !types[j].IsInterface && helperInterfaceType.IsAssignableFrom(types[j]))
                    {
                        typeList.Add(types[j].FullName);
                    }
                }
            }

            for (int i = 0; i < typeList.Count; i++)
            {
                if (typeList[i] == helperTypeName.stringValue)
                {
                    helperTypeIndex = i;
                }
            }
            
            return typeList.ToArray();
        }
    }
}
