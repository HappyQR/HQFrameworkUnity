using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HQFramework.Runtime
{
    public abstract partial class HQListBase : UIBehaviour
    {
        [SerializeField]
        protected RectTransform itemTemplate;

        [SerializeField]
        private UnityEvent<GameObject, int> _onInitItem;

        [SerializeField]
        private UnityEvent<string, GameObject, GameObject, int> _onItemButtonClick;

        [SerializeField]
        private UnityEvent<GameObject, int> _onSelectItem;

        [SerializeField]
        private UnityEvent<GameObject, int> _onUnselectItem;

        [SerializeField]
        private UnityEvent<GameObject, int> _onHoverEnterItem;

        [SerializeField]
        private UnityEvent<GameObject, int> _onHoverExitItem;

        [SerializeField]
        private UnityEvent _onOverHead;

        [SerializeField]
        private UnityEvent _onOverTail;

        public UnityEvent<GameObject, int> onInitItem => _onInitItem;

        public UnityEvent<string, GameObject, GameObject, int> onItemButtonClick => _onItemButtonClick;

        public UnityEvent<GameObject, int> onSelectItem => _onSelectItem;

        public UnityEvent<GameObject, int> onUnSelectItem => _onUnselectItem;

        public UnityEvent<GameObject, int> onHoverEnterItem => _onHoverEnterItem;

        public UnityEvent<GameObject, int> onHoverExitItem => _onHoverExitItem;

        public UnityEvent onOverHead => _onOverHead;

        public UnityEvent onOverTail => _onOverTail;

        protected override void Awake()
        {
            itemTemplate.gameObject.SetActive(false);
        }

        public abstract void SetItemCount(int count);

        public abstract void ScrollTo(int index);
    }
}
