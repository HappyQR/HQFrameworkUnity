using System;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class HQListLoopHorizontal : HQListLoopBase
    {
        public enum LayoutMode : byte
        {
            LeftToRight,
            RightToLeft
        }

        [SerializeField]
        private int numPerCol = 1;

        private int colCount;

        protected override void Awake()
        {
            base.Awake();
            rectTransform.pivot = Vector2.up;
            (itemTemplate.transform as RectTransform).pivot = Vector2.up;
        }

        protected override Vector2 CalculateListRectSize(int count)
        {
            colCount = Mathf.CeilToInt((float)count / numPerCol);
            Vector2 size = new Vector2(padding.left + padding.right + (itemTemplate.Width + horizontalSpacing) * colCount, rectTransform.sizeDelta.y);
            return size;
        }

        protected override IndexRange CalculateVisibleRange()
        {
            int minIndex = (int)((-rectTransform.anchoredPosition.x - padding.left) / (itemTemplate.Width + horizontalSpacing)) * numPerCol;
            int maxIndex = ((int)((-rectTransform.anchoredPosition.x + scrollRect.viewport.rect.width - padding.left) / (itemTemplate.Width + horizontalSpacing)) + 1) * numPerCol - 1;
            minIndex = Math.Max(minIndex, 0);
            maxIndex = Math.Min(maxIndex, ItemCount - 1);
            return new IndexRange(minIndex, maxIndex);
        }

        protected override Vector2 CalculateItemPositon(int index)
        {
            float x = padding.left + index / numPerCol * (itemTemplate.Width + horizontalSpacing);
            float y = -padding.top - index % numPerCol * (itemTemplate.Height + verticalSpacing);
            return new Vector2(x, y);
        }

        public override void ScrollTo(int index)
        {
            if (index < 0 || index > ItemCount)
            {
                throw new ArgumentOutOfRangeException($"index out of range. the list has {ItemCount} items, but you want to scroll to {index}");
            }
            Vector2 rect = rectTransform.rect.size;
            Vector2 expectPos = rect * ((float)index / ItemCount);
            rectTransform.anchoredPosition = new Vector2(-expectPos.x, rectTransform.anchoredPosition.y);
        }
    }
}
