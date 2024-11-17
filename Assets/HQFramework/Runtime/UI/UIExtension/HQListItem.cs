using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HQFramework.Runtime
{
    public class ListItem : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private RectTransform[] linkedElements;

        [SerializeField]
        private UnityEvent _onInit;

        [SerializeField]
        private UnityEvent _onSelect;

        [SerializeField]
        private UnityEvent _onUnselect;

        [SerializeField]
        private UnityEvent _onHoverEnter;

        [SerializeField]
        private UnityEvent _onHoverExit;

        private RectTransform rectTransform;
        private bool isSelected;

        public float Width => rectTransform.rect.width;
        public float Height => rectTransform.rect.height;

        protected override void Awake()
        {
            rectTransform = transform as RectTransform;
        }

        public RectTransform GetElement(int index)
        {
            return linkedElements[index];
        }

        public T GetUIControl<T>(int index) where T : UIBehaviour
        {
            return linkedElements[index].GetComponent<T>();
        }

        public void Init()
        {
            
        }

        public void Refresh()
        {
            _onInit.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            isSelected = !isSelected;
            if (isSelected)
            {
                _onSelect.Invoke();
            }
            else
            {
                _onUnselect.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _onHoverEnter.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _onHoverExit.Invoke();
        }
    }
}
