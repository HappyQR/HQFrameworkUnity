using UnityEngine;
using UnityEditor;
using System;
using System.Threading.Tasks;

namespace HQFramework.Editor
{
    public abstract class TabContentWindow : EditorWindow
    {
        private float tabPanelWidth = 200;
        private bool isResizing = false;
        private Rect tabPanelRect;
        private Rect contentPanelRect;
        private Rect splitterRect;
        private TabContentView[] contentList;
        private int currentTabIndex = 0;
        private int preTabIndex = 0;
        private Vector2 tabViewPos;
        private GUIStyle normalTab;
        private GUIStyle selectedTab;
        protected TabContentView currentContentView;

        private void OnGUI()
        {
            if (contentList == null)
                return;

            GUIStyle tabBackground = "RL Background";
            tabBackground.padding = new RectOffset(1, 2, 10, 5);
            tabPanelRect = new Rect(0, 0, tabPanelWidth, position.height);
            GUILayout.BeginArea(tabPanelRect, tabBackground);
            OnTabPanelGUI();
            GUILayout.EndArea();

            contentPanelRect = new Rect(tabPanelWidth + 5, 0, position.width - tabPanelWidth - 5, position.height);
            GUILayout.BeginArea(contentPanelRect, GUI.skin.box);
            OnContentPanelGUI();
            GUILayout.EndArea();

            splitterRect = new Rect(tabPanelWidth, 0, 5, position.height);
            EditorGUIUtility.AddCursorRect(splitterRect, MouseCursor.ResizeHorizontal);
            GUILayout.BeginArea(new Rect(tabPanelWidth, 0, 5, position.height));
            GUILayout.Box("", GUILayout.Width(5), GUILayout.Height(position.height));
            GUILayout.EndArea();

            HandleEvents();
        }

        private void HandleEvents()
        {
            Event currentEvent = Event.current;

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (splitterRect.Contains(currentEvent.mousePosition))
                    {
                        isResizing = true;
                        currentEvent.Use();
                    }
                    if (Event.current.button == 0)
                    {
                        GUI.FocusControl(null);
                    }
                    break;

                case EventType.MouseUp:
                    if (isResizing)
                    {
                        isResizing = false;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (isResizing)
                    {
                        tabPanelWidth = Mathf.Clamp(currentEvent.mousePosition.x, 100, position.width - 100);
                        Repaint();
                    }
                    break;
            }
        }

        protected virtual void OnTabPanelGUI()
        {
            tabViewPos = GUILayout.BeginScrollView(tabViewPos);
            tabViewPos.x = 0;
            for (int i = 0; i < contentList.Length; i++)
            {
                if (GUILayout.Button(contentList[i].TabTitle, currentTabIndex == i ? selectedTab : normalTab, GUILayout.Height(50)))
                {
                    if (preTabIndex != i)
                    {
                        contentList[preTabIndex].OnDisable();
                        contentList[i].OnEnable();
                        currentTabIndex = i;
                        currentContentView = contentList[i];
                        preTabIndex = currentTabIndex;
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        protected virtual void OnContentPanelGUI()
        {
            if (contentList.Length > currentTabIndex)
            {
                contentList[currentTabIndex].viewRect = contentPanelRect;
                contentList[currentTabIndex].OnGUI();
            }
        }

        protected virtual async void OnEnable()
        {
            GUIStyleState normalState = new GUIStyleState();
            normalState.background = Texture2D.blackTexture;
            normalState.textColor = Color.gray;
            GUIStyleState selectedState = new GUIStyleState();
            selectedState.background = Texture2D.grayTexture;
            selectedState.textColor = Color.white;

            normalTab = new GUIStyle();
            normalTab.fontSize = 12;
            normalTab.alignment = TextAnchor.MiddleLeft;
            normalTab.contentOffset = Vector2.right * 10;
            normalTab.normal = normalState;

            selectedTab = new GUIStyle();
            selectedTab.fontSize = 13;
            selectedTab.alignment = TextAnchor.MiddleLeft;
            selectedTab.contentOffset = Vector2.right * 10;
            selectedTab.normal = selectedState;

            await OnInitialize();

            InitializeContent(out contentList);
            if (currentContentView == null)
            {
                if (contentList.Length > 0)
                {
                    currentContentView = contentList[currentTabIndex];
                }
            }

            currentContentView?.OnEnable();
        }

        protected virtual void OnDisable()
        {
            currentContentView?.OnDisable();
        }
        
        protected abstract Task OnInitialize();
        protected abstract void InitializeContent(out TabContentView[] contentList);
    }

    public abstract class TabContentView
    {
        public Rect viewRect;
        private EditorWindow rootWindow;
        private GUIContent tabTitle;

        public EditorWindow RootWindow => rootWindow;
        public GUIContent TabTitle => tabTitle;

        public TabContentView(EditorWindow baseWindow, GUIContent tabTitle)
        {
            this.rootWindow = baseWindow;
            this.tabTitle = tabTitle;
        }

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnGUI() { }
    }
}
