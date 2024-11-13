using System;
using HQFramework.Resource;

namespace HQFramework.UI
{
    public interface IUIManager
    {
        void SetResourceManager(IResourceManager resourceManager);

        void SetHelper(IUIHelper helper);

        void AddUIGroup(IUIGroup group);

        void OpenUIForm(Type formType, Action<IUIForm> onComplete, Action<string> onError);

        void OpenUIForm<T>(Action<IUIForm> onComplete, Action<string> onError) where T : class, IUIForm, new();

        void CloseUIForm(Type formType);

        void CloseUIForm(IUIForm form);

        void CloseUIForm<T>() where T : class, IUIForm, new();

        IUIForm GetUIForm(Type formType);

        T GetUIForm<T>() where T : class, IUIForm, new();

        bool HasUIForm(Type formType);

        bool HasUIForm<T>() where T : class, IUIForm, new();
    }
}
