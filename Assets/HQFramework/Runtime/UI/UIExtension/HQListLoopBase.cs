using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HQFramework.Runtime
{
    public abstract partial class HQListLoopBase : HQListBase
    {
        protected struct IndexRange
        {
            public readonly int minIndex;
            public readonly int maxIndex;

            public IndexRange(int minIndex, int maxIndex)
            {
                this.minIndex = minIndex;
                this.maxIndex = maxIndex;
            }
        }

        [SerializeField]
        protected ScrollRect scrollRect;

        [SerializeField]
        private float scrollRecalculateThreshold = 0.1f;

        [SerializeField]
        protected float verticalSpacing = 5f;

        [SerializeField]
        protected float horizontalSpacing = 5f;

        [SerializeField]
        protected RectOffset padding;

        private int count;
        private bool dirty;
        private IndexRange preVisibleRange;
        private Vector2 previousPosition;
        private ListPool pool;
        private Dictionary<int, HQListItem> visibleItemDic;
        private HashSet<int> refreshPendingSet;

        public int ItemCount => count;

        protected override void Awake()
        {
            base.Awake();
            pool = new ListPool(this);
            visibleItemDic = new Dictionary<int, HQListItem>();
            refreshPendingSet = new HashSet<int>();
            pool.Recyle(itemTemplate);
            scrollRect.onValueChanged.AddListener(OnListScroll);
        }

        public override void SetItemCount(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("Item count must not be negative.");
            }
            this.count = count;
            rectTransform.sizeDelta = CalculateListRectSize(this.count);

            foreach (int index in visibleItemDic.Keys)
            {
                refreshPendingSet.Add(index);
            }

            dirty = true;
        }

        public override void AppendItemCount(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("Item count must not be negative.");
            }
            this.count += count;
            rectTransform.sizeDelta = CalculateListRectSize(this.count);

            dirty = true;
        }

        public override void InsertItem(int index)
        {
            this.count += 1;
            rectTransform.sizeDelta = CalculateListRectSize(this.count);
            foreach (int i in visibleItemDic.Keys)
            {
                if (i >= index)
                {
                    refreshPendingSet.Add(i);
                }
            }

            dirty = true;
        }

        public override void RemoveItem(int index)
        {
            this.count -= 1;
            rectTransform.sizeDelta = CalculateListRectSize(this.count);
            foreach (int i in visibleItemDic.Keys)
            {
                if (i >= index)
                {
                    refreshPendingSet.Add(i);
                }
            }

            dirty = true;
        }

        public override void RefershItem(int index)
        {
            if (visibleItemDic.ContainsKey(index))
            {
                refreshPendingSet.Add(index);
            }

            dirty = true;
        }

        private void OnListScroll(Vector2 normalizedValue)
        {
            Vector2 deltaVector = rectTransform.anchoredPosition - previousPosition;
            if (deltaVector.sqrMagnitude < scrollRecalculateThreshold)
            {
                return;
            }
            previousPosition = rectTransform.anchoredPosition;
            dirty = true;
        }

        private void LateUpdate()
        {
            if (!dirty)
            {
                return;
            }

            IndexRange currentVisibleRange = CalculateVisibleRange();

            for (int i = preVisibleRange.minIndex; i < currentVisibleRange.minIndex; i++)
            {
                if (visibleItemDic.ContainsKey(i))
                {
                    if (visibleItemDic[i] != null)
                        pool.Recyle(visibleItemDic[i]);
                    visibleItemDic.Remove(i);
                    refreshPendingSet.Remove(i);
                }
            }

            for (int i = currentVisibleRange.maxIndex + 1; i <= preVisibleRange.maxIndex; i++)
            {
                if (visibleItemDic.ContainsKey(i))
                {
                    if (visibleItemDic[i] != null)
                        pool.Recyle(visibleItemDic[i]);
                    visibleItemDic.Remove(i);
                    refreshPendingSet.Remove(i);
                }
            }

            preVisibleRange = currentVisibleRange;

            for (int i = currentVisibleRange.minIndex; i <= currentVisibleRange.maxIndex; i++)
            {
                int index = i;
                if (visibleItemDic.ContainsKey(index))
                {
                    if (refreshPendingSet.Contains(index))
                    {
                        visibleItemDic[index].name = $"{itemName}-{index}";
                        visibleItemDic[index].Init(index);
                        refreshPendingSet.Remove(index);
                    }
                    continue;
                }
                HQListItem item = pool.Spawn(index);
                item.Init(index);
                item.transform.localPosition = CalculateItemPositon(index);
                visibleItemDic.Add(index, item);
            }

            dirty = false;
        }

        protected abstract Vector2 CalculateListRectSize(int count);

        protected abstract IndexRange CalculateVisibleRange();

        protected abstract Vector2 CalculateItemPositon(int index);
    }
}
