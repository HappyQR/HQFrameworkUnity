using UnityEngine;

namespace HQFramework.Runtime
{
    public abstract partial class HQListLoopBase : HQListBase
    {
        [SerializeField]
        protected HQScrollRect scrollRect;

        [SerializeField]
        protected float verticalSpacing;

        [SerializeField]
        protected float horizontalSpacing;

        [SerializeField]
        protected RectOffset padding;

        protected RectTransform rectTransform;
        protected ListPool listPool;
        protected int count;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = transform as RectTransform;
            listPool = new ListPool(this);
            listPool.Recyle(itemTemplate);
            scrollRect.ScrollEvent.AddListener(CalculateVisibleRange);
        }

        public override void SetItemCount(int count)
        {
            this.count = count;
            rectTransform.sizeDelta = CalculateListRect(count);
        }

        protected abstract Vector2 CalculateListRect(int count);

        protected abstract void CalculateVisibleRange(Vector2 delta);
    }
}
