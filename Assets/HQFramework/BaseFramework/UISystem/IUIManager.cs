using System;

namespace HQFramework.UI
{
    public interface IUIManager
    {
        void ShowForm(Type formType, Action<IUIForm> callback);

        void HideForm(Type formType, Action callback);

        void HideForm(IUIForm form);

        IUIForm GetForm(Type formType);
    }
}
