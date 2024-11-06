using UnityEngine;
using UnityEngine.EventSystems;

namespace HQFramework.Runtime
{
    public abstract class WindowBase
    {
        protected GameObject gameObject;
        protected Transform transform;

        private WindowLinkHolder linkHolder;
        private Canvas canvas;

        public abstract uint Crc
        {
            get;
        }

        public T GetUI<T>(int index) where T : UIBehaviour
        {
            return linkHolder.linkedElementList[index].GetComponent<T>();
        }

        public RectTransform GetElement(int index)
        {
            return linkHolder.linkedElementList[index];
        }

        internal void BindInstance(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
            this.canvas = gameObject.GetComponent<Canvas>();
            this.linkHolder = gameObject.GetComponent<WindowLinkHolder>();
        }

        protected internal virtual void OnCreate()
        {

        }

        protected internal virtual void OnShow()
        {

        }

        protected internal virtual void OnUpdate()
        {

        }

        protected internal virtual void OnHide()
        {

        }

        protected internal virtual void OnDestroy()
        {

        }
    }
}
