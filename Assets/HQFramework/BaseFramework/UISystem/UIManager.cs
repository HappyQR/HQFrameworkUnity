using System;
using System.Collections.Generic;
using HQFramework.Resource;

namespace HQFramework.UI
{
    internal class UIManager : HQModuleBase, IUIManager
    {
        public override byte Priority => byte.MaxValue;

        private IResourceManager resourceManager;
        private IUIHelper helper;

        private Dictionary<int, IUIGroup> groupDic;
        private Dictionary<Type, IUIForm> formDic;
        private LinkedList<IUIForm> visibleFormList;
        private Queue<IUIForm> disposePendingQueue;

        protected override void OnInitialize()
        {
            groupDic = new Dictionary<int, IUIGroup>();
            formDic = new Dictionary<Type, IUIForm>();
            visibleFormList = new LinkedList<IUIForm>();
            disposePendingQueue = new Queue<IUIForm>();
        }

        protected override void OnUpdate()
        {
            if (disposePendingQueue.Count > 0)
            {
                IUIForm form = disposePendingQueue.Dequeue();
                if (!form.Alive)
                {
                    form.OnDestroy();
                    resourceManager.ReleaseAsset(form.FormObject);
                }
                else
                {
                    disposePendingQueue.Enqueue(form);
                }
            }

            for (LinkedListNode<IUIForm> formNode = visibleFormList.First; formNode != null; formNode = formNode.Next)
            {
                IUIForm form = formNode.Value;
                if (form.Covered)
                {
                    if (!form.PauseOnCovered)
                    {
                        form.OnUpdate();
                    }
                }
                else
                {
                    form.OnUpdate();
                }
            }
        }

        public void SetResourceManager(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public void SetHelper(IUIHelper helper)
        {
            this.helper = helper;
        }

        public void AddUIGroup(IUIGroup group)
        {
            if (groupDic.ContainsKey(group.GroupID))
            {
                HQDebugger.LogError($"There's already {group.GroupID} group here, you can't add it again.");
                return;
            }
            groupDic.Add(group.GroupID, group);
        }

        public void OpenUIForm(Type formType, object userData, Action<IUIForm> onComplete, Action<string> onError)
        {
            if (formDic.ContainsKey(formType))
            {
                IUIForm form = formDic[formType];
                if (form.Visible)
                {
                    HQDebugger.LogError($"The UIForm {formType} is opening, you can't do it again.");
                    return;
                }
                else
                {
                    groupDic[form.GroupID].OnFormOpened(form);
                    form.SetVisible(true);
                    form.OnOpen(userData);
                    visibleFormList.AddLast(form);
                }
            }
            else
            {
                IUIForm form = Activator.CreateInstance(formType) as IUIForm;
                if (!groupDic.ContainsKey(form.GroupID))
                {
                    onError?.Invoke($"There's no {form.GroupID} here, you may need to add the UIGroup first.");
                    return;
                }

                void OnInstantiateFormComplete(IUIFormLinker linker)
                {
                    formDic.Add(formType, form);
                    visibleFormList.AddLast(form);
                    form.OnCreate(linker);
                    helper.AttachFormToGroup(form, groupDic[form.GroupID]);
                    groupDic[form.GroupID].OnFormOpened(form);
                    form.SetVisible(true);
                    form.OnOpen(userData);
                    onComplete?.Invoke(form);
                }

                void OnInstantiateFormError(string errorMessage)
                {
                    onError?.Invoke(errorMessage);
                }

                helper.InstantiateForm(form, OnInstantiateFormComplete, OnInstantiateFormError);
            }
        }

        public void OpenUIForm<T>(object userData, Action<IUIForm> onComplete, Action<string> onError) where T : class, IUIForm, new()
        {
            OpenUIForm(typeof(T), userData, onComplete, onError);
        }

        public void CloseUIForm(Type formType)
        {
            if(!formDic.ContainsKey(formType))
            {
                HQDebugger.LogError($"There's no UIForm {formType} opening.");
                return;
            }
            else
            {
                IUIForm form = formDic[formType];
                if (!form.Visible)
                {
                    HQDebugger.LogError($"The UIForm {formType} is closing, you can't do it again.");
                    return;
                }
                form.OnClose();
                form.SetVisible(false);
                visibleFormList.Remove(form);
                groupDic[form.GroupID].OnFormClosed(form);
                if (form.DestroyOnClose)
                {
                    formDic.Remove(formType);
                    disposePendingQueue.Enqueue(form);
                }
            }
        }

        public void CloseUIForm(IUIForm form)
        {
            CloseUIForm(form.GetType());
        }

        public void CloseUIForm<T>() where T : class, IUIForm, new()
        {
            CloseUIForm(typeof(T));
        }

        public IUIForm GetUIForm(Type formType)
        {
            formDic.TryGetValue(formType, out IUIForm form);
            return form;
        }

        public T GetUIForm<T>() where T : class, IUIForm, new()
        {
            return (T)GetUIForm(typeof(T));
        }

        public bool HasUIForm(Type formType)
        {
            return formDic.ContainsKey(formType);
        }

        public bool HasUIForm<T>() where T : class, IUIForm, new()
        {
            return formDic.ContainsKey(typeof(T));
        }

        public void DeleteUIGroup(int groupID)
        {
            groupDic.Remove(groupID);
            Queue<IUIForm> closeQueue = new Queue<IUIForm>();
            foreach (IUIForm form in groupDic.Values)
            {
                if (form.GroupID == groupID)
                {
                    closeQueue.Enqueue(form);
                }
            }

            while (closeQueue.Count > 0)
            {
                IUIForm form = closeQueue.Dequeue();
                formDic.Remove(form.GetType());
                if (form.Visible)
                {
                    visibleFormList.Remove(form);
                    form.OnClose();
                }
                form.OnDestroy();
                resourceManager.ReleaseAsset(form.FormObject);
            }
        }

        public void DeleteUIGroup(IUIGroup group)
        {
            DeleteUIGroup(group.GroupID);
        }
    }
}
