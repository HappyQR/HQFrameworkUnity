using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public partial class AssetArchiveView : TabContentView
    {
        private List<List<AssetModuleCompileInfo>> moduleList;
        private Vector2[] moduleScrollPosList;
        private Vector2 scrollPos;
        private Dictionary<int, int> selectedModuleDic;
        private List<List<GUIContent>> moduleBuildBtnContentList;
        private GUIStyle moduleTitleStyle;
        private GUIStyle normalBtnStyle;
        private GUIStyle selectedBtnStyle;

        public AssetArchiveView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override async void OnEnable()
        {
            GUIStyleState normalState = new GUIStyleState();
            normalState.background = Texture2D.blackTexture;
            normalState.textColor = new Color(1, 1, 1, 0.6f);
            moduleTitleStyle = new GUIStyle();
            moduleTitleStyle.fontSize = 14;
            moduleTitleStyle.alignment = TextAnchor.MiddleLeft;
            moduleTitleStyle.contentOffset = new Vector2(10, 25);
            moduleTitleStyle.normal = normalState;

            moduleList = new List<List<AssetModuleCompileInfo>>();
            moduleBuildBtnContentList = new List<List<GUIContent>>();
            selectedModuleDic = new Dictionary<int, int>();
            Dictionary<int, List<AssetModuleCompileInfo>> tempModuleDic = new Dictionary<int, List<AssetModuleCompileInfo>>();
            List<AssetModuleCompileInfo> historyData = await HQAssetBuildLauncher.GetAssetModuleCompileHistoryAsync();
            foreach (var item in historyData)
            {
                if (!tempModuleDic.ContainsKey(item.moduleID))
                {
                    tempModuleDic.Add(item.moduleID, new List<AssetModuleCompileInfo>());
                }
                tempModuleDic[item.moduleID].Add(item);
            }
            foreach (var item in tempModuleDic)
            {
                moduleList.Add(item.Value);
            }
            moduleList.Sort((item1, item2) => item1[0].moduleID < item2[0].moduleID ? -1 : 1);
            moduleScrollPosList = new Vector2[moduleList.Count];
            for (int i = 0; i < moduleScrollPosList.Length; i++)
            {
                moduleScrollPosList[i].x = int.MaxValue;
                selectedModuleDic.Add(i, moduleList[i].Count - 1);

                moduleBuildBtnContentList.Add(new List<GUIContent>(moduleList[i].Count));
                for (int j = 0; j < moduleList[i].Count; j++)
                {
                    AssetModuleCompileInfo moduleResult = moduleList[i][j];
                    GUIContent btnContent = new GUIContent($"Build Code: {moduleResult.buildVersionCode}");
                    StringBuilder toolTip = new StringBuilder();
                    toolTip.Append("Build Time: ");
                    toolTip.Append(moduleResult.buildTime);
                    toolTip.Append("\n\n");
                    toolTip.Append("Dev Notes: ");
                    toolTip.Append(moduleResult.devNotes);
                    toolTip.Append("\n\n");
                    toolTip.Append("Dependencies: ");
                    for (int k = 0; k < moduleResult.dependencies.Length; k++)
                    {
                        toolTip.Append(moduleResult.dependencies[k]);
                        if (k < moduleResult.dependencies.Length - 1)
                            toolTip.Append(", ");
                    }
                    btnContent.tooltip = toolTip.ToString();
                    moduleBuildBtnContentList[i].Add(btnContent);
                }
            }
        }

        public override void OnGUI()
        {
            if (normalBtnStyle == null)
            {
                normalBtnStyle = "AppToolbarButtonRight";
                selectedBtnStyle = "ButtonRight";
            }
            
            GUILayout.BeginArea(new Rect(10, 10, viewRect.width - 20, viewRect.height - 10));
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            scrollPos.x = 0;
            for (int i = 0; i < moduleList.Count; i++)
            {
                DrawMoudleHistory(i, moduleList[i]);
                GUILayout.Space(5);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();


            GUILayout.FlexibleSpace();

            GUIContent btnContent = EditorGUIUtility.IconContent("d_Package Manager");
            btnContent.text = "Archive";
            if (GUILayout.Button(btnContent, GUILayout.Height(45)))
            {
                List<AssetModuleCompileInfo> results = new List<AssetModuleCompileInfo>();
                foreach (var item in selectedModuleDic)
                {
                    results.Add(moduleList[item.Key][item.Value]);
                }
                EditorApplication.delayCall += () => ArchiveNotesWindow.ShowWindow(results);
            }
            GUILayout.Space(10);
            GUILayout.EndArea();
        }

        private void DrawDivider()
        {
            Color originalColor = GUI.color;
            GUI.color = Color.black;
            GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.Width(2));
            GUI.color = originalColor;
        }

        private void DrawMoudleHistory(int index, List<AssetModuleCompileInfo> moduleBuildResultList)
        {
            GUIStyle backgroundStyle = "DD Background";
            GUILayout.BeginHorizontal(backgroundStyle, GUILayout.Height(80));

            GUIContent moduleContent = EditorGUIUtility.IconContent("d_UnityLogo");
            moduleContent.text = $" {moduleBuildResultList[0].moduleName}\n id: {moduleBuildResultList[0].moduleID}";
            GUILayout.Box(moduleContent, moduleTitleStyle, GUILayout.Width(100), GUILayout.Height(30));
            DrawDivider();
            GUILayout.Space(3);

            GUILayout.BeginVertical();
            GUILayout.Space(7.5f);
            moduleScrollPosList[index] = GUILayout.BeginScrollView(moduleScrollPosList[index], GUIStyle.none, GUIStyle.none);
            moduleScrollPosList[index].y = 0;
            GUILayout.BeginHorizontal();
            GUILayout.Space(3);
            for (int i = 0; i < moduleBuildResultList.Count; i++)
            {
                GUIStyle btnStyle = selectedModuleDic.ContainsKey(index) && selectedModuleDic[index] == i ? selectedBtnStyle : normalBtnStyle;
                if (GUILayout.Button(moduleBuildBtnContentList[index][i], btnStyle, GUILayout.Width(100), GUILayout.Height(60)))
                {
                    if (selectedModuleDic.ContainsKey(index))
                    {
                        if (selectedModuleDic[index] == i)
                        {
                            selectedModuleDic.Remove(index);
                        }
                        else
                        {
                            selectedModuleDic[index] = i;
                        }
                    }
                    else
                    {
                        selectedModuleDic.Add(index, i);
                    }
                }
                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        public override void OnDisable()
        {
            moduleList = null;
            moduleScrollPosList = null;
            moduleBuildBtnContentList = null;
        }
    }
}
