using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetModuleManager
    {
        private static readonly string assetsModuleConfigDir = "Assets/Config/EditorConfig/AssetModule/";
        public static readonly string manifestFileName = "AssetModuleManifest.json";

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
                (AssetModuleManifest currentManifest, AssetModuleManifest builtinManifest) = AssetBuildUtility.GetCurrentAssetsManifest();
                if (currentManifest != null && currentManifest.moduleDic != null && currentManifest.moduleDic.ContainsKey(id))
                {
                    AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultOption();
                    currentManifest.moduleDic.Remove(id);
                    string manifestInfo = JsonUtilityEditor.ToJson(currentManifest);
                    string manifestFilePath = Path.Combine(buildOption.manifestOutputDir, manifestFileName);
                    File.WriteAllText(manifestFilePath, manifestInfo);
                }

                if (module.isBuiltin)
                {
                    if (builtinManifest != null && builtinManifest.moduleDic != null && builtinManifest.moduleDic.ContainsKey(id))
                    {
                        AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultOption();
                        builtinManifest.moduleDic.Remove(id);
                        string manifestInfo = JsonUtilityEditor.ToJson(builtinManifest);
                        string manifestFilePath = Path.Combine(buildOption.builtinDir, manifestFileName);
                        File.WriteAllText(manifestFilePath, manifestInfo);
                    }
                }

                Debug.Log("delete assets module successfully : " + moduleName);
            }
            else
            {
                Debug.LogError("delete assets module error : " + moduleName);
            }
            return result;
        }

        // 模块划分规范约束
        // 1.检查各模块之间的依赖，若某一模块存在对其它模块的依赖，抛出错误
        // 2.检查模块配置
        // public static bool CheckAllModulesFormat()
        // {
        //     List<AssetModuleConfig> modules = GetModuleList();
        //     bool result = true;
        //     for (int i = 0; i < modules.Count; i++)
        //     {
        //         bool passFormatCheck = true;
        //         AssetModuleConfig module = modules[i];
        //         if (string.IsNullOrEmpty(module.name))
        //         {
        //             Debug.LogError($"There is a module without name : module ID : {module.id}");
        //             passFormatCheck = false;
        //         }

        //         if (module.rootFolder == null)
        //         {
        //             Debug.LogError($"There is a module without root reference : module ID : {module.id}, module name : {module.moduleName}");
        //             passFormatCheck = false;
        //         }
        //         else
        //         {
        //             string rootDir = AssetDatabase.GetAssetPath(module.rootFolder);
        //             string[] assets = AssetDatabase.FindAssets("", new string[] { rootDir });
        //             if (assets == null || assets.Length == 0)
        //             {
        //                 Debug.LogError($"There is a module without assets : module ID : {module.id}, module name : {module.moduleName}");
        //                 passFormatCheck = false;
        //             }
        //         }

        //         if (passFormatCheck)
        //         {
        //             AssetBundleBuild[] builds = InitializeModuleBuild(module);
        //             string moduleNamePrefix = module.moduleName.ToLower();
        //             for (int j = 0; j < builds.Length; j++)
        //             {
        //                 string[] bundleDependencies = AssetDatabase.GetAssetBundleDependencies(builds[j].assetBundleName, true);
        //                 for (int k = 0; k < bundleDependencies.Length; k++)
        //                 {
        //                     string targetModuleNamePrefix = bundleDependencies[k].Split('_')[0];
        //                     if (targetModuleNamePrefix != moduleNamePrefix)
        //                     {
        //                         Debug.LogError($"There is a module depend to other module : module ID : {module.id}, module name : {module.moduleName}, dependency : {targetModuleNamePrefix}");
        //                         passFormatCheck = false;
        //                     }
        //                 }
        //             }
        //         }

        //         result = passFormatCheck && result;
        //     }

        //     if (result)
        //     {
        //         Debug.Log("Asset Modules Check Done, Clean.");
        //     }

        //     return result;
        // }
    }
}
