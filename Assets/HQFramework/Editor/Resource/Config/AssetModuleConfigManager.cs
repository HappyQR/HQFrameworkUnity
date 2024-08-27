using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetModuleConfigManager
    {
        private static readonly string assetsModuleConfigDir = "Assets/Configuration/Editor/Asset/AssetModule/";

        public static int GetNewModuleID()
        {
            List<AssetModuleConfig> modules = GetModuleList();
            if (modules.Count == 0)
            {
                return 0;
            }

            modules.Sort((module1, module2) => module1.id > module2.id ? -1 : 1);
            return modules[0].id + 1;
        }

        public static bool CreateNewAssetModule(AssetModuleConfig module)
        {
            string fileName = module.moduleName;
            if (string.IsNullOrEmpty(module.moduleName))
            {
                fileName = "NewAssetModule";
            }
            module.createTimeTicks = DateTime.Now.Ticks;
            EditorUtility.SetDirty(module);
            AssetDatabase.SaveAssetIfDirty(module);
            string result = AssetDatabase.RenameAsset(assetsModuleConfigDir + "temp.asset", fileName + ".asset");
            if (!string.IsNullOrEmpty(result))
            {
                Debug.LogError(result);
                return false;
            }
            else
            {
                Debug.Log("successfully create new assets module : " + module.moduleName);
                return true;
            }
        }

        public static List<AssetModuleConfig> GetModuleList()
        {
            List<AssetModuleConfig> modules = new List<AssetModuleConfig>();
            if (!AssetDatabase.IsValidFolder(assetsModuleConfigDir))
            {
                Directory.CreateDirectory(FileUtilityEditor.GetPhysicalPath(assetsModuleConfigDir));
                AssetDatabase.Refresh();
            }
            string[] configs = AssetDatabase.FindAssets("", new[] { assetsModuleConfigDir });
            for (var i = 0; i < configs.Length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(configs[i]);
                try
                {
                    AssetModuleConfig module = AssetDatabase.LoadAssetAtPath<AssetModuleConfig>(filePath);
                    module.createTime = new DateTime(module.createTimeTicks);
                    modules.Add(module);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Debug.LogError("Don't put other object under assets module config directory!");
                }
            }
            modules.Sort((module1, module2) => module1.createTimeTicks < module2.createTimeTicks ? -1 : 1);
            return modules;
        }

        public static bool DeleteAssetModule(AssetModuleConfig module)
        {
            string moduleName = module.moduleName;
            int id = module.id;
            bool confirm = EditorUtility.DisplayDialog("Delete Assets Module", $"are you sure to delete the assets module : {moduleName}?", "Delete", "Cancel");
            if (!confirm)
                return false;
            bool result = AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(module));
            if (result)
            {
                Debug.Log("delete assets module successfully : " + moduleName);
            }
            else
            {
                Debug.LogError("delete assets module error : " + moduleName);
            }
            return result;
        }
    }
}
