using System;

namespace HQFramework.UI
{
    public interface IUIHelper
    {
        void InstantiateForm(IUIForm form, Action<IUIFormLinker> onComplete, Action<string> onError);
        void AttachFormToGroup(IUIForm form, IUIGroup group);
    }
}
