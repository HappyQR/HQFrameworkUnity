using System.Collections.Generic;
using System.Linq;
using System.Text;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetPublishView : TabContentView
    {
        private List<AssetArchiveData> archiveDataList;
        private AssetModuleManifest lastPublishManifest;
        private int selectedArchiveIndex = -1;

        private GUIStyle normalBtnStyle;
        private GUIStyle selectedBtnStyle;
        private GUIStyle moduleTitleStyle;
        private GUIContent archiveIcon;
        private Vector2 archiveScrollPos;
        private Vector2 genericScrollPos;
        private Vector2 modulesScrollPos;

        private int pageIndex = 1;

        private string releaseNote;
        private string resourceVersion;
        private int versionCode;
        private int minimalSupportedVersionCode;
        private Dictionary<int, string> moduleReleaseNotesDic;
        private Dictionary<int, int> moduleMinimalSupportedVersionDic;
        private List<AssetModuleCompileInfo> moduleList;

        public AssetPublishView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override async void OnEnable()
        {
            List<AssetModuleManifest> history = await HQAssetBuildLauncher.GetAssetPublishHistoryAsync();
            archiveDataList = await HQAssetBuildLauncher.GetAssetArchivesAsync();
            archiveIcon = EditorGUIUtility.IconContent("SceneSet Icon");
            archiveScrollPos.y = int.MaxValue;

            if (history != null && history.Count > 0)
            {
                lastPublishManifest = history.Last();
                releaseNote = lastPublishManifest.releaseNote;
                resourceVersion = lastPublishManifest.resourceVersion;
                versionCode = lastPublishManifest.versionCode + 1;
                minimalSupportedVersionCode = lastPublishManifest.minimalSupportedVersionCode;
            }

            GUIStyleState normalState = new GUIStyleState();
            normalState.background = Texture2D.blackTexture;
            normalState.textColor = new Color(1, 1, 1, 0.85f);
            moduleTitleStyle = new GUIStyle();
            moduleTitleStyle.fontSize = 15;
            moduleTitleStyle.alignment = TextAnchor.MiddleLeft;
            moduleTitleStyle.normal = normalState;
        }

        public override void OnGUI()
        {
            if (archiveDataList == null)
                return;
            GUILayout.BeginArea(new Rect(10, 10, viewRect.width - 20, viewRect.height - 10));
            
            switch (pageIndex)
            {
                case 1:
                    PickArchive();
                    break;
                case 2:
                    EditGenericInfo();
                    break;
                case 3:
                    EditModulesInfo();
                    break;
                case 4:
                    ConfirmPublish();
                    break;
            }
            
            GUILayout.EndArea();
        }

        private void PickArchive()
        {
            GUIStyle headerStyle = "AM HeaderStyle";
            if (normalBtnStyle == null)
            {
                normalBtnStyle = "AppToolbarButtonRight";
                selectedBtnStyle = "ButtonRight";
            }

            GUILayout.BeginVertical();
            archiveScrollPos = GUILayout.BeginScrollView(archiveScrollPos);
            archiveScrollPos.x = 0;

            for (int i = 0; i < archiveDataList.Count; i++)
            {
                GUIStyle btnStyle = selectedArchiveIndex == i ? selectedBtnStyle : normalBtnStyle;
                GUILayout.BeginHorizontal();
                GUILayout.Space(12);
                GUIContent btnContent = new GUIContent();
                btnContent.tooltip = archiveDataList[i].archiveNotes;
                if (GUILayout.Button(btnContent, btnStyle, GUILayout.Height(75), GUILayout.Width(viewRect.width - 50)))
                {
                    if (i == selectedArchiveIndex)
                    {
                        selectedArchiveIndex = -1;
                    }
                    else
                    {
                        selectedArchiveIndex = i;
                    }
                }
                GUI.Box(new Rect(40, i * 88f + 10, 75, 75), archiveIcon, GUIStyle.none);
                GUI.Label(new Rect((viewRect.width - 50) / 2 - 85, i * 88f + 20, viewRect.width - 100, 20), $"Archive Time:\t{archiveDataList[i].archiveTime}", headerStyle);
                GUI.Label(new Rect((viewRect.width - 50) / 2 - 85, i * 88f + 40, viewRect.width - 100, 20), $"Archive Tag:\t{archiveDataList[i].archiveTag}", headerStyle);
                
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            if (GUILayout.Button("<- Previous", GUILayout.Width(150), GUILayout.Height(45)))
            {
                pageIndex--;
            }
            GUI.enabled = selectedArchiveIndex != -1;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next ->", GUILayout.Width(150), GUILayout.Height(45)))
            {
                pageIndex++;
                ResetExtraModules();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        private void EditGenericInfo()
        {
            GUIStyle headerStyle = "AM HeaderStyle";
            AssetArchiveData selectedArchive = archiveDataList[selectedArchiveIndex];
            GUILayout.BeginVertical();
            genericScrollPos = GUILayout.BeginScrollView(genericScrollPos);
            genericScrollPos.x = 0;

            GUILayout.Label($"Selected Archive : {selectedArchive.archiveTag}", headerStyle);
            GUILayout.Label($"Archive Time : {selectedArchive.archiveTime}", headerStyle);
            GUILayout.Label("Archive Notes: ", headerStyle);
            GUILayout.Space(3);
            GUILayout.Label(selectedArchive.archiveNotes);

            GUILayout.Space(20);

            GUILayout.Label("Resource Version : ", headerStyle);
            GUILayout.Space(5);
            resourceVersion = EditorGUILayout.TextField(resourceVersion);
            GUILayout.Space(10);
            GUILayout.Label("Internal Version Code : ", headerStyle);
            GUILayout.Space(5);
            versionCode = EditorGUILayout.IntField(versionCode);
            GUILayout.Space(10);
            GUILayout.Label("Minimal Supported Version Code : ", headerStyle);
            GUILayout.Space(5);
            minimalSupportedVersionCode = EditorGUILayout.IntField(minimalSupportedVersionCode);
            GUILayout.Space(10);
            GUILayout.Label("Release Notes : ", headerStyle);
            GUILayout.Space(5);
            releaseNote = EditorGUILayout.TextArea(releaseNote, GUILayout.Height(150));
            GUILayout.EndScrollView();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<- Previous", GUILayout.Width(150), GUILayout.Height(45)))
            {
                pageIndex--;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next ->", GUILayout.Width(150), GUILayout.Height(45)))
            {
                pageIndex++;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        private void EditModulesInfo()
        {
            GUIStyle headerStyle = "AM HeaderStyle";
            GUIStyle backgroundStyle = "DD Background";
            AssetArchiveData selectedArchive = archiveDataList[selectedArchiveIndex];
            GUILayout.BeginVertical();
            modulesScrollPos = GUILayout.BeginScrollView(modulesScrollPos);
            modulesScrollPos.x = 0;
            for (int i = 0; i < moduleList.Count; i++)
            {
                AssetModuleCompileInfo module = moduleList[i];
                GUILayout.BeginHorizontal(backgroundStyle, GUILayout.Height(150));
                GUILayout.BeginVertical(GUILayout.Width(200));
                GUILayout.BeginHorizontal(GUILayout.Height(65));
                GUIContent moduleContent = EditorGUIUtility.IconContent("d_UnityLogo");
                moduleContent.tooltip = module.devNotes;
                GUILayout.Box(moduleContent, moduleTitleStyle, GUILayout.Width(65));
                GUILayout.Space(5);
                GUILayout.BeginVertical();
                GUILayout.Space(12);
                GUILayout.Label(module.moduleName, moduleTitleStyle, GUILayout.Height(20f));
                GUILayout.Label($"ID: {module.moduleID}", moduleTitleStyle, GUILayout.Height(20f));
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                StringBuilder str = new StringBuilder();
                str.Append("[");
                for (int j = 0; j < module.dependencies.Length; j++)
                {
                    str.Append(module.dependencies[j]);
                    if (j != module.dependencies.Length - 1)
                    {
                        str.Append(", ");
                    }
                }
                str.Append("]");
                GUILayout.Label($"Dependencies : {str}", headerStyle);
                GUILayout.Label($"Build Version Code : {module.buildVersionCode}", headerStyle);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Minimal Supported Version Code: ", headerStyle);
                moduleMinimalSupportedVersionDic[module.moduleID] = EditorGUILayout.IntField(moduleMinimalSupportedVersionDic[module.moduleID]);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Label($"Build Time : {module.buildTime}", headerStyle);
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Space(2);
                GUILayout.Label("Release Notes: ", headerStyle);
                GUILayout.Space(3);
                moduleReleaseNotesDic[module.moduleID] = EditorGUILayout.TextArea(moduleReleaseNotesDic[module.moduleID], GUILayout.ExpandHeight(true));
                GUILayout.EndVertical();               
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            GUILayout.EndScrollView();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<- Previous", GUILayout.Width(150), GUILayout.Height(45)))
            {
                pageIndex--;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next ->", GUILayout.Width(150), GUILayout.Height(45)))
            {
                pageIndex++;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        private void ConfirmPublish()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginArea(new Rect(10, 10, viewRect.width - 20, viewRect.height - 60));

            

            GUILayout.EndArea();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<- Previous", GUILayout.Width(150), GUILayout.Height(45)))
            {
                pageIndex--;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Publish", GUILayout.Width(150), GUILayout.Height(45)))
            {
                
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        private void ResetExtraModules()
        {
            AssetArchiveData selectedArchive = archiveDataList[selectedArchiveIndex];
            moduleList = new List<AssetModuleCompileInfo>();
            moduleMinimalSupportedVersionDic = new Dictionary<int, int>();
            moduleReleaseNotesDic = new Dictionary<int, string>();
            for (int i = 0; i < selectedArchive.moduleCompileInfoList.Count; i++)
            {
                AssetModuleCompileInfo moduleCompileInfo = selectedArchive.moduleCompileInfoList[i];
                if (moduleCompileInfo.isBuiltin)
                {
                    continue;
                }

                moduleList.Add(moduleCompileInfo);
                if (lastPublishManifest != null && lastPublishManifest.moduleDic.ContainsKey(moduleCompileInfo.moduleID))
                {
                    moduleMinimalSupportedVersionDic.Add(moduleCompileInfo.moduleID, lastPublishManifest.moduleDic[moduleCompileInfo.moduleID].minimalSupportedPatchVersion);
                    moduleReleaseNotesDic.Add(moduleCompileInfo.moduleID, lastPublishManifest.moduleDic[moduleCompileInfo.moduleID].releaseNote);
                }
                else
                {
                    moduleMinimalSupportedVersionDic.Add(moduleCompileInfo.moduleID, 0);
                    moduleReleaseNotesDic.Add(moduleCompileInfo.moduleID, string.Empty);
                }
            }
        }
    }
}
