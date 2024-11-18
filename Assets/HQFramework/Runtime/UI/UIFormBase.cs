using System;
using System.Collections;
using HQFramework.Coroutine;
using HQFramework.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HQFramework.Runtime
{
    public abstract class UIFormBase : IUIForm
    {
        protected UIFormLinker formLinker;
        protected CanvasGroup canvasGroup;
        protected bool alive;
        private bool visible;
        private bool covered;

        public abstract string AssetPath { get; }

        public abstract uint AssetCrc { get; }

        public abstract int GroupID { get; }

        public bool Visible => visible;

        public bool Alive => alive;

        public bool Covered => covered;

        public object FormObject => formLinker.FormObject;

        public bool DestroyOnClose => formLinker.DestroyOnClose;

        public bool PauseOnCovered => formLinker.PauseOnCovered;

        protected RectTransform GetUIElement(int index)
        {
            return formLinker.linkedElements[index];
        }

        protected T GetUIControl<T>(int index) where T : UIBehaviour
        {
            RectTransform item = formLinker.linkedElements[index];
            return item.GetComponent<T>();
        }

        protected UIBehaviour GetUIControl(int index, Type type)
        {
            RectTransform item = formLinker.linkedElements[index];
            return (UIBehaviour)item.GetComponent(type);
        }

        protected virtual void OnCreate(){}

        protected virtual void OnOpen(object userData){}

        protected virtual void OnUpdate(){}

        protected virtual void OnCovered(){}

        protected virtual void OnRevealed(){}

        protected virtual void OnClose(){}

        protected virtual void OnDestroy(){}

        protected virtual void OnButtonClick(string buttonName){}

        protected virtual void OnToggleValueChanged(string toggleName, bool value){}

        protected virtual void OnSliderValueChanged(string sliderName, float value){}

        protected virtual void OnListItemInit(string listName, HQListItem item, int index){}

        protected virtual void OnListItemSelected(string listName, HQListItem item, int index){}

        protected virtual void OnListItemUnselected(string listName, HQListItem item, int index){}

        protected virtual void OnListItemHoverEnter(string listName, HQListItem item, int index){}

        protected virtual void OnListItemHoverExit(string listName, HQListItem item, int index){}

        protected virtual void OnListItemButtonClick(string listName, HQListItem item, string buttonName, GameObject buttonObject, int index){}

        protected virtual void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 0 : 1;
            canvasGroup.interactable = !visible;
            if (visible)
            {
                HQFrameworkEngine.GetModule<ICoroutineManager>().StartCoroutine(FadeIn());
            }
            else
            {
                HQFrameworkEngine.GetModule<ICoroutineManager>().StartCoroutine(FadeOut());
            }
        }

        private IEnumerator FadeIn()
        {
            WaitForSecondsRealtime waiter = new WaitForSecondsRealtime(0.02f);
            while (canvasGroup.alpha < 1)
            {
                if (!visible)
                {
                    yield break;
                }
                canvasGroup.alpha += 0.01f;
                yield return waiter;
            }

            canvasGroup.interactable = true;
        }

        private IEnumerator FadeOut()
        {
            WaitForSecondsRealtime waiter = new WaitForSecondsRealtime(0.02f);
            while (canvasGroup.alpha > 0)
            {
                if (visible)
                {
                    yield break;
                }
                canvasGroup.alpha -= 0.01f;
                yield return waiter;
            }

            canvasGroup.interactable = false;
            if (DestroyOnClose)
            {
                alive = false;
            }
        }

        void IUIForm.OnCreate(IUIFormLinker linker)
        {
            alive = true;
            formLinker = linker as UIFormLinker;
            canvasGroup = formLinker.GetComponent<CanvasGroup>();
            for (int i = 0; i < formLinker.linkedElements.Length; i++)
            {
                RectTransform item = formLinker.linkedElements[i];

                if (item.TryGetComponent<Button>(out Button button))
                {
                    button.onClick.AddListener(() => OnButtonClick(button.name));
                }

                if (item.TryGetComponent<Toggle>(out Toggle toggle))
                {
                    toggle.onValueChanged.AddListener((value) => OnToggleValueChanged(toggle.name, value));
                }

                if (item.TryGetComponent<Slider>(out Slider slider))
                {
                    slider.onValueChanged.AddListener((value) => OnSliderValueChanged(slider.name, value));
                }

                if (item.TryGetComponent<HQListBase>(out HQListBase list))
                {
                    list.onInitItem += OnListItemInit;
                    list.onSelectItem += OnListItemSelected;
                    list.onUnselectItem += OnListItemUnselected;
                    list.onHoverEnterItem += OnListItemHoverEnter;
                    list.onHoverExitItem += OnListItemHoverExit;
                    list.onItemButtonClick += OnListItemButtonClick;
                }
            }
            OnCreate();
        }

        void IUIForm.OnOpen(object userData)
        {
            OnOpen(userData);
        }

        void IUIForm.OnUpdate()
        {
            OnUpdate();
        }

        void IUIForm.OnCovered()
        {
            covered = true;
            OnCovered();
        }

        void IUIForm.OnRevealed()
        {
            covered = false;
            OnRevealed();
        }

        void IUIForm.OnClose()
        {
            OnClose();
        }

        void IUIForm.OnDestroy()
        {
            OnDestroy();
        }

        void IUIForm.SetVisible(bool visible)
        {
            this.visible = visible;
            canvasGroup.blocksRaycasts = visible;
            SetVisible(visible);
        }
    }
}
