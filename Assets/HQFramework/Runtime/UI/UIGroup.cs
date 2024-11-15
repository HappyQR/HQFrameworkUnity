using System.Collections.Generic;
using HQFramework.UI;
using UnityEngine;

namespace HQFramework.Runtime
{
    public sealed class UIGroup : MonoBehaviour, IUIGroup
    {
        [SerializeField]
        private int groupID;

        [SerializeField]
        private int depth;

        private LinkedList<IUIForm> formList = new LinkedList<IUIForm>();

        public int GroupID => groupID;

        public object GroupRoot => gameObject;

        public int Depth => depth;

        public void OnFormOpened(IUIForm form)
        {
            (form.FormObject as GameObject).transform.SetAsLastSibling();
            LinkedListNode<IUIForm> formNode = new LinkedListNode<IUIForm>(form);
            formList.AddLast(formNode);
            LinkedListNode<IUIForm> coveredFormNode = formNode.Previous;
            if (coveredFormNode != null)
            {
                coveredFormNode.Value.OnCovered();
            }
        }

        public void OnFormClosed(IUIForm form)
        {
            LinkedListNode<IUIForm> formNode = formList.Find(form);
            LinkedListNode<IUIForm> revealedFormNode = formNode.Previous;
            if (revealedFormNode != null)
            {
                revealedFormNode.Value.OnRevealed();
            }
            formList.Remove(formNode);
        }
    }
}
