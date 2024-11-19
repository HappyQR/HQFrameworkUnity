using System;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class HQListLoopVertical : HQListLoopBase
    {
        public enum LayoutMode : byte
        {
            TopToBottom,
            BottomToTop
        }

        public enum AlignMode : byte
        {
            UpperLeft,
            UpperCenter,
            LowerLeft,
            LowerCenter
        }

        [SerializeField]
        private int numPerRow = 1;

        private int rowCount;

        protected override void Awake()
        {
            base.Awake();
            rectTransform.pivot = Vector2.up;
            (itemTemplate.transform as RectTransform).pivot = Vector2.up;
        }

        protected override Vector2 CalculateListRectSize(int count)
        {
            rowCount = Mathf.CeilToInt((float)count / numPerRow);
            Vector2 size = new Vector2(rectTransform.sizeDelta.x, padding.top + padding.bottom + (itemTemplate.Height + verticalSpacing) * rowCount);
            return size;
        }

        protected override IndexRange CalculateVisibleRange()
        {
            int minIndex = (int)((rectTransform.anchoredPosition.y - padding.top) / (itemTemplate.Height + verticalSpacing)) * numPerRow;
            int maxIndex = ((int)((rectTransform.anchoredPosition.y + scrollRect.viewport.rect.height - padding.top) / (itemTemplate.Height + verticalSpacing)) + 1) * numPerRow - 1;
            minIndex = Math.Max(minIndex, 0);
            maxIndex = Math.Min(maxIndex, ItemCount - 1);
            return new IndexRange(minIndex, maxIndex);
        }

        protected override Vector2 CalculateItemPositon(int index)
        {
            float x = padding.left + index % numPerRow * (itemTemplate.Width + horizontalSpacing);
            float y = -padding.top - index / numPerRow * (itemTemplate.Height + verticalSpacing);
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
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, expectPos.y);
        }
    }
}
