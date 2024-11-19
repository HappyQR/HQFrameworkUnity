using System.Collections.Generic;
using UnityEngine;

namespace HQFramework.Runtime
{
    public partial class HQListLoopBase
    {
        protected sealed class ListPool
        {
            private HQListLoopBase list;

            private Stack<HQListItem> itemStack;

            public ListPool(HQListLoopBase list)
            {
                this.list = list;
                itemStack = new Stack<HQListItem>();
            }

            public HQListItem Spawn(int index)
            {
                HQListItem item = null;
                if (itemStack.Count > 0)
                {
                    item = itemStack.Pop();
                }
                else
                {
                    GameObject itemObject = Instantiate(list.itemTemplate.gameObject, list.transform);
                    item = itemObject.GetComponent<HQListItem>();
                    list.BindItemEvents(item);
                }
                item.name = $"{list.itemName}-{index}";
                item.SetVisible(true);
                return item;
            }

            public void Recyle(HQListItem item)
            {
                item.SetVisible(false);
                itemStack.Push(item);
            }
        }
    }
}
