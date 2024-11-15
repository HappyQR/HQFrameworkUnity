using System;
using System.Collections.Generic;
using System.Reflection;
using HQFramework.Resource;
using HQFramework.Runtime;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    [CustomEditor(typeof(ResourceComponent))]
    public class ResourceComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty resourceHelperTypeName;
        private SerializedProperty hotfixMode;
        private SerializedProperty launcherHotfixID;
        private SerializedProperty assetsPersistentDir;
        private SerializedProperty assetsBuiltinDir;
        private SerializedProperty hotfixManifestUrl;

        private HQHotfixMode hotfixModeProxy;

        private string[] helperTypeList;
        private int helperTypeIndex;

        private void OnEnable()
        {
            resourceHelperTypeName = serializedObject.FindProperty(nameof(resourceHelperTypeName));
            hotfixMode = serializedObject.FindProperty(nameof(hotfixMode));
            launcherHotfixID = serializedObject.FindProperty(nameof(launcherHotfixID));
            assetsPersistentDir = serializedObject.FindProperty(nameof(assetsPersistentDir));
            assetsBuiltinDir = serializedObject.FindProperty(nameof(assetsBuiltinDir));
            hotfixManifestUrl = serializedObject.FindProperty(nameof(hotfixManifestUrl));

            hotfixModeProxy = (HQHotfixMode)hotfixMode.enumValueIndex;

            helperTypeList = CollectResourceHelperTypes();
        }

        public override void OnInspectorGUI()
        {
            GUIStyle headerStyle = "AM HeaderStyle";
            GUILayout.BeginHorizontal();
            GUILayout.Label("Resource Helper: ", headerStyle);
            helperTypeIndex = EditorGUILayout.Popup(helperTypeIndex, helperTypeList, GUILayout.ExpandWidth(true));
            resourceHelperTypeName.stringValue = helperTypeList[helperTypeIndex];
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hotfix Mode: ", headerStyle);
            hotfixModeProxy = (HQHotfixMode)EditorGUILayout.EnumPopup(hotfixModeProxy, GUILayout.ExpandWidth(true));
            hotfixMode.enumValueIndex = (int)hotfixModeProxy;
            GUILayout.EndHorizontal();

            if (hotfixModeProxy != HQHotfixMode.NoHotfix)
            {
                EditorGUILayout.Separator();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Launcher Hotfix ID: ", headerStyle);
                launcherHotfixID.intValue = EditorGUILayout.IntField(launcherHotfixID.intValue, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Separator();
            GUILayout.Label("Assets Persistent Dir:(related to Application.persistentDataPath)", headerStyle);
            assetsPersistentDir.stringValue = EditorGUILayout.TextField(assetsPersistentDir.stringValue);

            EditorGUILayout.Separator();
            GUILayout.Label("Assets Built-in Dir:(related to Application.streamingAssetsPath)", headerStyle);
            assetsBuiltinDir.stringValue = EditorGUILayout.TextField(assetsBuiltinDir.stringValue);

            EditorGUILayout.Separator();
            GUILayout.Label("Hotfix Manifest URL:", headerStyle);
            hotfixManifestUrl.stringValue = EditorGUILayout.TextField(hotfixManifestUrl.stringValue);

            serializedObject.ApplyModifiedProperties();
        }

        private string[] CollectResourceHelperTypes()
        {
            List<string> typeList = new List<string>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type helperInterfaceType = typeof(IResourceHelper);
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
                if (typeList[i] == resourceHelperTypeName.stringValue)
                {
                    helperTypeIndex = i;
                }
            }
            
            return typeList.ToArray();
        }
    }
}
