using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HQFramework.Runtime
{
    public class HQScrollRect : ScrollRect
    {
        private UnityEvent<Vector2> onScroll;

        public UnityEvent<Vector2> ScrollEvent => onScroll;

        public override void OnScroll(PointerEventData data)
        {
            base.OnScroll(data);
            onScroll.Invoke(data.scrollDelta);
        }
    }
}
