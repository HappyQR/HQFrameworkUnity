using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public class AssetArchiveView : TabContentView
    {
        private List<List<AssetModuleBuildResult>> moduleList;
        private Vector2[] moduleScrollPosList;
        private Vector2 scrollPos;
        private Dictionary<int, int> selectedModuleDic;
        private GUIStyle moduleTitleStyle;
        private GUIStyle middleCenterVerticalStyle;

        public AssetArchiveView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override async void OnEnable()
        {
            GUIStyleState normalState = new GUIStyleState();
            normalState.background = Texture2D.blackTexture;
            normalState.textColor = new Color(1, 1, 1, 0.6f);
            moduleTitleStyle = new GUIStyle();
            moduleTitleStyle.fontSize = 15;
            moduleTitleStyle.alignment = TextAnchor.MiddleLeft;
            moduleTitleStyle.contentOffset = new Vector2(10, 25);
            moduleTitleStyle.normal = normalState;

            moduleList = new List<List<AssetModuleBuildResult>>();
            selectedModuleDic = new Dictionary<int, int>();
            AssetModuleBuildHistoryData historyData = await AssetArchiver.LoadBuildHistoryData();
            foreach (var item in historyData.moduleBuildData)
            {
                moduleList.Add(item.Value);
            }

            moduleList.Sort((item1, item2) => item1[0].moduleID < item2[0].moduleID ? -1 : 1);
            moduleScrollPosList = new Vector2[moduleList.Count];
            for (int i = 0; i < moduleScrollPosList.Length; i++)
            {
                moduleScrollPosList[i].x = int.MaxValue;
                selectedModuleDic.Add(i, moduleList[i].Count - 1);
            }
        }

        public override void OnGUI()
        {
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

            if (GUILayout.Button("Archive", GUILayout.Height(45)))
            {

            }
            GUILayout.Space(10);
            GUILayout.EndArea();
        }

        private void DrawMoudleHistory(int index, List<AssetModuleBuildResult> moduleBuildResultList)
        {
            GUIStyle backgroundStyle = "PopupCurveSwatchBackground";
            GUILayout.BeginHorizontal(backgroundStyle, GUILayout.Height(80));

            GUIContent moduleContent = EditorGUIUtility.IconContent("d_UnityLogo");
            moduleContent.text = $" {moduleBuildResultList[0].moduleName}";
            GUILayout.Box(moduleContent, moduleTitleStyle, GUILayout.Width(90), GUILayout.Height(30));

            GUILayout.Space(10);

            GUILayout.BeginVertical();
            GUILayout.Space(7.5f);
            moduleScrollPosList[index] = GUILayout.BeginScrollView(moduleScrollPosList[index], GUIStyle.none, GUIStyle.none);
            moduleScrollPosList[index].y = 0;
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            for (int i = 0; i < moduleBuildResultList.Count; i++)
            {
                AssetModuleBuildResult moduleResult = moduleBuildResultList[i];
                GUIStyle btnStyle = "AppToolbarButtonRight";
                if (selectedModuleDic.ContainsKey(index) && selectedModuleDic[index] == i)
                {
                    btnStyle = "ButtonRight";
                }
                if (GUILayout.Button($"Build Code: {moduleResult.buildVersionCode}", btnStyle, GUILayout.Width(100), GUILayout.Height(60)))
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
        }
    }
}
