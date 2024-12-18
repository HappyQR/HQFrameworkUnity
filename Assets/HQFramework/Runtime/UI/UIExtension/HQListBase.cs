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

        protected string itemName;
        protected string listName;
        protected RectTransform rectTransform;
        protected Action<string, HQListItem, int> _onInitItem;
        protected Action<string, HQListItem, int> _onClickItem;
        protected Action<string, HQListItem, int> _onHoverEnterItem;
        protected Action<string, HQListItem, int> _onHoverExitItem;
        protected Action<string, HQListItem, string, GameObject, int> _onItemButtonClick;

        public event Action<string, HQListItem, int> onInitItem { add => _onInitItem += value; remove => _onInitItem -= value; }
        public event Action<string, HQListItem, int> onClickItem { add => _onClickItem += value; remove => _onClickItem -= value; }
        public event Action<string, HQListItem, int> onHoverEnterItem { add => _onHoverEnterItem += value; remove => onHoverEnterItem -= value; }
        public event Action<string, HQListItem, int> onHoverExitItem { add => _onHoverExitItem += value; remove => _onHoverExitItem -= value; }
        public event Action<string, HQListItem, string, GameObject, int> onItemButtonClick { add => _onItemButtonClick += value; remove => _onItemButtonClick -= value; }

        protected override void Awake()
        {
            rectTransform = transform as RectTransform;
            BindItemEvents(itemTemplate);
            itemName = itemTemplate.name;
            listName = this.name;
        }

        protected void BindItemEvents(HQListItem listItem)
        {
            listItem.onInit.AddListener((item, index) => _onInitItem.Invoke(listName, item, index));
            listItem.onClick.AddListener((item, index) => _onClickItem.Invoke(listName, item, index));
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
