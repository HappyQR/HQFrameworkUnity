using System;
using HQFramework.Resource;
using HQFramework.UI;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class UIHelper : IUIHelper
    {
        private ResourceComponent resourceComponent;

        public UIHelper()
        {
            resourceComponent = GameEntry.GetModule<ResourceComponent>();
        }

        public void AttachFormToGroup(IUIForm form, IUIGroup group)
        {
            Transform formTransform = (form.FormObject as GameObject).transform;
            Transform groupTransform = (group.GroupRoot as GameObject).transform;
            formTransform.SetParent(groupTransform);

            formTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            formTransform.localScale = Vector3.one;
            (formTransform as RectTransform).offsetMax = Vector2.zero;
            (formTransform as RectTransform).offsetMin = Vector2.zero;
        }

        public void InstantiateForm(IUIForm form, Action<IUIFormLinker> onComplete, Action<string> onError)
        {
            void OnInstantiateFormComplete(ResourceLoadCompleteEventArgs<GameObject> args)
            {
                IUIFormLinker linker = args.asset.GetComponent<IUIFormLinker>();
                onComplete.Invoke(linker);
            }

            void OnInstantiateFormError(ResourceLoadErrorEventArgs args)
            {
                onError?.Invoke(args.errorMessage);
            }
            
            resourceComponent.InstantiateAsset<GameObject>(form.AssetCrc, OnInstantiateFormComplete, OnInstantiateFormError, 0, 0);
        }
    }
}
