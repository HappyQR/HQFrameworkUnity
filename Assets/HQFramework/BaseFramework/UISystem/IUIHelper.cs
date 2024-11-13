using System;

namespace HQFramework.UI
{
    public interface IUIHelper
    {
        void InstantiateForm(IUIForm form, Action<object> onComplete, Action<string> onError);
        void AttachFormToGroup(IUIForm form, IUIGroup group);
    }
}
