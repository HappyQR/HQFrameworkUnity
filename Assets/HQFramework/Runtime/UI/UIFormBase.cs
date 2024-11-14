using System;
using HQFramework.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HQFramework.Runtime
{
    public abstract class UIFormBase : IUIForm
    {
        protected UIFormLinker formLinker;
        protected bool visible;
        protected bool alive;
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

        protected virtual void OnCreate(){}

        protected virtual void OnOpen(object userData){}

        protected virtual void OnUpdate(){}

        protected virtual void OnCovered(){}

        protected virtual void OnRevealed(){}

        protected virtual void OnClose(){}

        protected virtual void OnDestroy(){}

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

        protected virtual void OnButtonClick(string buttonName)
        {

        }

        void IUIForm.OnCreate(IUIFormLinker linker)
        {
            formLinker = linker as UIFormLinker;
            for (int i = 0; i < formLinker.linkedElements.Length; i++)
            {
                RectTransform item = formLinker.linkedElements[i];

                if (item.TryGetComponent<Button>(out Button button))
                {
                    button.onClick.AddListener(() => OnButtonClick(button.name));
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
        }
    }
}
