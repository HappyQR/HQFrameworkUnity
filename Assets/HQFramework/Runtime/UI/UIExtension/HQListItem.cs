using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HQFramework.Runtime
{
    public class HQListItem : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private RectTransform[] linkedElements;

        [SerializeField]
        private UnityEvent<HQListItem, int> _onInit;

        [SerializeField]
        private UnityEvent<HQListItem, int> _onClick;

        [SerializeField]
        private UnityEvent<HQListItem, int> _onHoverEnter;

        [SerializeField]
        private UnityEvent<HQListItem, int> _onHoverExit;

        [SerializeField]
        private UnityEvent<HQListItem, string, GameObject, int> _onButtonClick;

        private RectTransform rectTransform;
        private List<Button> buttonList;
        private int index;

        public float Width => rectTransform.rect.width;
        public float Height => rectTransform.rect.height;
        public UnityEvent<HQListItem, int> onInit => _onInit;
        public UnityEvent<HQListItem, int> onClick => _onClick;
        public UnityEvent<HQListItem, int> onHoverEnter => _onHoverEnter;
        public UnityEvent<HQListItem, int> onHoverExit => _onHoverExit;
        public UnityEvent<HQListItem, string, GameObject, int> onButtonClick => _onButtonClick;

        protected override void Awake()
        {
            rectTransform = transform as RectTransform;
            buttonList = new List<Button>();
            for (int i = 0; i < linkedElements.Length; i++)
            {
                if (linkedElements[i].TryGetComponent<Button>(out Button button))
                {
                    buttonList.Add(button);
                }
            }
            for (int i = 0; i < buttonList.Count; i++)
            {
                Button button = buttonList[i];
                button.onClick.AddListener(() => _onButtonClick.Invoke(this, button.name, button.gameObject, index));
            }
        }

        public RectTransform GetElement(int index)
        {
            return linkedElements[index];
        }

        public T GetUIControl<T>(int index) where T : UIBehaviour
        {
            return linkedElements[index].GetComponent<T>();
        }

        internal void Init(int index)
        {
            this.index = index;
            _onInit.Invoke(this, index);
        }

        internal void Refresh()
        {
            _onInit.Invoke(this, index);
        }

        internal void SetVisible(bool visible)
        {
            transform.localScale = visible ? Vector3.one : Vector3.zero;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            _onClick.Invoke(this, index);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _onHoverEnter.Invoke(this, index);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _onHoverExit.Invoke(this, index);
        }
    }
}
