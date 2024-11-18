using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HQFramework.Runtime
{
    public abstract partial class HQListBase : UIBehaviour
    {
        [SerializeField]
        protected HQListItem itemTemplate;

        [SerializeField]
        protected UnityEvent _onOverHead;

        [SerializeField]
        protected UnityEvent _onOverTail;

        protected string itemName;
        protected string listName;
        protected Action<string, HQListItem, int> _onInitItem;
        protected Action<string, HQListItem, int> _onSelectItem;
        protected Action<string, HQListItem, int> _onUnselectItem;
        protected Action<string, HQListItem, int> _onHoverEnterItem;
        protected Action<string, HQListItem, int> _onHoverExitItem;
        protected Action<string, HQListItem, string, GameObject, int> _onItemButtonClick;


        public UnityEvent onOverHead => _onOverHead;
        public UnityEvent onOverTail => _onOverTail;
        public event Action<string, HQListItem, int> onInitItem { add => _onInitItem += value; remove => _onInitItem -= value; }
        public event Action<string, HQListItem, int> onSelectItem { add => _onSelectItem += value; remove => _onSelectItem -= value; }
        public event Action<string, HQListItem, int> onUnselectItem { add => _onUnselectItem += value; remove => _onUnselectItem -= value; }
        public event Action<string, HQListItem, int> onHoverEnterItem { add => _onHoverEnterItem += value; remove => onHoverEnterItem -= value; }
        public event Action<string, HQListItem, int> onHoverExitItem { add => _onHoverExitItem += value; remove => _onHoverExitItem -= value; }
        public event Action<string, HQListItem, string, GameObject, int> onItemButtonClick { add => _onItemButtonClick += value; remove => _onItemButtonClick -= value; }

        protected override void Awake()
        {
            BindItemEvents(itemTemplate);
            itemName = itemTemplate.name;
            listName = this.name;
        }

        protected void BindItemEvents(HQListItem listItem)
        {
            listItem.onInit.AddListener((item, index) => _onInitItem.Invoke(listName, item, index));
            listItem.onSelect.AddListener((item, index) => _onSelectItem.Invoke(listName, item, index));
            listItem.onUnselect.AddListener((item, index) => _onUnselectItem.Invoke(listName, item, index));
            listItem.onHoverEnter.AddListener((item, index) => _onHoverEnterItem.Invoke(listName, item, index));
            listItem.onHoverExit.AddListener((item, index) => _onHoverExitItem.Invoke(listName, item, index));
            listItem.onButtonClick.AddListener((item, btnName, btnObject, index) => _onItemButtonClick.Invoke(listName, item, btnName, btnObject, index));
        }

        public abstract void SetItemCount(int count);

        public abstract void AppendItemCount(int count);

        public abstract void InsertItem(int index);

        public abstract void RemoveItem(int index);

        public abstract void RefershItem(int index);

        public abstract void ScrollTo(int index);
    }
}
