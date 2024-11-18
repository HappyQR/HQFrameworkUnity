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

            public HQListItem Spawn()
            {
                if (itemStack.Count > 0)
                {
                    return itemStack.Pop();
                }
                GameObject itemObject = Instantiate(list.itemTemplate.gameObject, list.transform);
                HQListItem item = itemObject.GetComponent<HQListItem>();
                list.BindItemEvents(item);
                return item;
            }

            public void Recyle(HQListItem item)
            {
                itemStack.Push(item);
            }
        }
    }
}
