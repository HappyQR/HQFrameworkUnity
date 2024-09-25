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

        public AssetArchiveView(EditorWindow baseWindow, GUIContent tabTitle) : base(baseWindow, tabTitle)
        {
        }

        public override async void OnEnable()
        {
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
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            scrollPos.x = 0;
            for (int i = 0; i < moduleList.Count; i++)
            {
                DrawMoudleHistory(i, moduleList[i]);
                GUILayout.Space(5);
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10);

            if (GUILayout.Button("Archive", GUILayout.Height(45)))
            {

            }

            GUILayout.EndVertical();
        }

        private void DrawMoudleHistory(int index, List<AssetModuleBuildResult> moduleBuildResultList)
        {
            GUILayout.BeginHorizontal("PopupCurveSwatchBackground", GUILayout.Height(60));

            GUIContent moduleContent = EditorGUIUtility.IconContent("d_UnityLogo");
            moduleContent.text = moduleBuildResultList[0].moduleName;
            GUILayout.Box(moduleContent);

            GUILayout.Space(10);

            moduleScrollPosList[index] = GUILayout.BeginScrollView(moduleScrollPosList[index]);
            moduleScrollPosList[index].y = 0;
            GUILayout.BeginHorizontal();
            for (int i = 0; i < moduleBuildResultList.Count; i++)
            {
                AssetModuleBuildResult moduleResult = moduleBuildResultList[i];
                GUIStyle btnStyle = "AppToolbarButtonRight";
                if (selectedModuleDic.ContainsKey(index) && selectedModuleDic[index] == i)
                {
                    btnStyle = "AppToolbarButtonMid";
                }
                if (GUILayout.Button(moduleResult.buildVersionCode.ToString(), btnStyle, GUILayout.Width(100), GUILayout.Height(60)))
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
                    GUILayout.Space(5);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }

        public override void OnDisable()
        {
            moduleList = null;
            moduleScrollPosList = null;
        }
    }
}
