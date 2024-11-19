using System;
using System.Collections.Generic;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class HQListCommon : HQListBase
    {
        public enum LayoutMode : byte
        {
            Vertical,
            Horizontal,
        }

        [SerializeField]
        private LayoutMode layoutMode;
        private LinkedList<HQListItem> itemList;

        protected override void Awake()
        {
            base.Awake();
            itemList = new LinkedList<HQListItem>();
            itemTemplate.gameObject.name = $"{itemName}-0";
            itemTemplate.gameObject.SetActive(false);
        }

        public override void SetItemCount(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("Item count must not be negative.");
            }
            else if (count > 128)
            {
                HQDebugger.LogWarning("Item count more than 128, maybe consider in using loop list.");
            }

            if (count < itemList.Count)
            {
                while (itemList.Count > count)
                {
                    LinkedListNode<HQListItem> node = itemList.Last;
                    if (node.Value == itemTemplate)
                    {
                        node.Value.gameObject.SetActive(false);
                    }
                    else
                    {
                        Destroy(node.Value.gameObject);
                    }
                    itemList.RemoveLast();
                }
            }
            else if (count > itemList.Count)
            {
                int index = itemList.Count;
                while (itemList.Count < count)
                {
                    HQListItem item = null;
                    if (index == 0)
                    {
                        itemTemplate.gameObject.SetActive(true);
                        item = itemTemplate;
                    }
                    else
                    {
                        item = CreateNewItem(index);
                    }
                    itemList.AddLast(item);
                    index++;
                }
            }
            
            int loopIndex = 0;
            for (LinkedListNode<HQListItem> node = itemList.First; node != null; node = node.Next)
            {
                node.Value.Init(loopIndex);
                loopIndex++;
            }
        }

        public override void InsertItem(int index)
        {
            if (index < 0 || index > itemList.Count)
            {
                throw new ArgumentOutOfRangeException($"index out of range. the list has {itemList.Count} items, but you want insert at {index}.");
            }

            if (index == 0 && itemList.Count == 0)
            {
                itemTemplate.gameObject.SetActive(true);
                itemList.AddLast(itemTemplate);
                itemTemplate.Init(index);
                return;
            }

            HQListItem newItem = CreateNewItem(index);
            LinkedListNode<HQListItem> node = itemList.First;
            int loopCount = index;
            while (loopCount > 0)
            {
                node = node.Next;
                loopCount--;
            }
            itemList.AddBefore(node, newItem);
            newItem.transform.SetSiblingIndex(index);
            newItem.Init(index);
            loopCount = index + 1;
            while (node != null)
            {
                node.Value.name = $"{itemName}-{loopCount}";
                node.Value.Init(loopCount);
                node = node.Next;
                loopCount++;
            }
        }

        public override void RemoveItem(int index)
        {
            if (index < 0 || index > itemList.Count)
            {
                throw new ArgumentOutOfRangeException($"index out of range. the list has {itemList.Count} items, but you want to remove at {index}");
            }
            if (itemList.Count == 0)
                return;
            LinkedListNode<HQListItem> node = itemList.First;
            int loopCount = index;
            while (loopCount > 0)
            {
                node = node.Next;
                loopCount--;
            }
            LinkedListNode<HQListItem> nextNode = null;
            if (node.Value == itemTemplate && itemList.Count > 1)
            {
                nextNode = node;
                node = node.Next;
                Destroy(node.Value.gameObject);
                itemList.Remove(node);
            }
            else if (node.Value == itemTemplate)
            {
                nextNode = null;
                node.Value.gameObject.SetActive(false);
                itemList.Remove(node);
            }
            else
            {
                nextNode = node.Next;
                Destroy(node.Value.gameObject);
                itemList.Remove(node);
            }
            loopCount = index;
            while (nextNode != null)
            {
                nextNode.Value.name = $"{itemName}-{loopCount}";
                nextNode.Value.Init(loopCount);
                loopCount++;
                nextNode = nextNode.Next;
            }
        }

        public override void AppendItemCount(int count)
        {
            if (count < 0)
            {
                throw new InvalidOperationException("Can not append negative count to list.");
            }

            int index = itemList.Count;
            while (count > 0)
            {
                HQListItem newItem = null;
                if (index == 0)
                {
                    itemTemplate.gameObject.SetActive(true);
                    newItem = itemTemplate;
                }
                else
                {
                    newItem = CreateNewItem(index);
                }
                itemList.AddLast(newItem);
                newItem.Init(index);
                count--;
                index++;
            }
        }

        public override void RefershItem(int index)
        {
            if (index < 0 || index > itemList.Count)
            {
                throw new ArgumentOutOfRangeException($"index out of range. the list has {itemList.Count} items, but you want to remove at {index}");
            }

            LinkedListNode<HQListItem> node = itemList.First;
            while (index > 0)
            {
                node = node.Next;
                index--;
            }
            node.Value.Refresh();
        }

        public override void ScrollTo(int index)
        {
            if (index < 0 || index > itemList.Count)
            {
                throw new ArgumentOutOfRangeException($"index out of range. the list has {itemList.Count} items, but you want to scroll to {index}");
            }
            Vector2 rect = rectTransform.rect.size;
            Vector2 expectPos = rect * ((float)index / itemList.Count);
            switch (layoutMode)
            {
                case LayoutMode.Vertical:
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, expectPos.y);
                    break;
                case LayoutMode.Horizontal:
                    rectTransform.anchoredPosition = new Vector2(expectPos.x, rectTransform.anchoredPosition.y);
                    break;
            }
        }

        private HQListItem CreateNewItem(int index)
        {
            GameObject itemObject = Instantiate<GameObject>(itemTemplate.gameObject, this.transform);
            itemObject.name = $"{itemName}-{index}";
            HQListItem newItem = itemObject.GetComponent<HQListItem>();
            BindItemEvents(newItem);
            return newItem;
        }
    }
}
