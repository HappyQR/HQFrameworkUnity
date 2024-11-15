using System;
using HQFramework.Resource;
using HQFramework.UI;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class UIComponent : BaseComponent
    {
        [SerializeField]
        private string helperTypeName;

        private IUIManager uiManager;

        private void Start()
        {
            InitializeUIHelper();
            IResourceManager resourceManager = HQFrameworkEngine.GetModule<IResourceManager>();
            uiManager.SetResourceManager(resourceManager);
        }

        private void InitializeUIHelper()
        {
            Type helperType = Utility.Assembly.GetType(helperTypeName);
            IUIHelper helper = (IUIHelper)Activator.CreateInstance(helperType);

            uiManager = HQFrameworkEngine.GetModule<IUIManager>();
            uiManager.SetHelper(helper);
        }

        public void AddUIGroup(IUIGroup group)
        {
            uiManager.AddUIGroup(group);
        }

        public void DeleteUIGroup(IUIGroup group)
        {
            uiManager.DeleteUIGroup(group);
        }

        public void DeleteUIGroup(int groupID)
        {
            uiManager.DeleteUIGroup(groupID);
        }

        public void OpenUIForm(Type formType, object userData, Action<IUIForm> onComplete, Action<string> onError)
        {
            uiManager.OpenUIForm(formType, userData, onComplete, onError);
        }

        public void OpenUIForm<T>(object userData, Action<IUIForm> onComplete, Action<string> onError) where T : class, IUIForm, new()
        {
            uiManager.OpenUIForm<T>(userData, onComplete, onError);
        }

        public void CloseUIForm(Type formType)
        {
            uiManager.CloseUIForm(formType);
        }

        public void CloseUIForm(IUIForm form)
        {
            uiManager.CloseUIForm(form);
        }

        public void CloseUIForm<T>() where T : class, IUIForm, new()
        {
            uiManager.CloseUIForm<T>();
        }

        public IUIForm GetUIForm(Type formType)
        {
            return uiManager.GetUIForm(formType);
        }

        public T GetUIForm<T>() where T : class, IUIForm, new()
        {
            return uiManager.GetUIForm<T>();
        }

        public bool HasUIForm(Type formType)
        {
            return uiManager.HasUIForm(formType);
        }

        public bool HasUIForm<T>() where T : class, IUIForm, new()
        {
            return uiManager.HasUIForm<T>();
        }
    }
}
