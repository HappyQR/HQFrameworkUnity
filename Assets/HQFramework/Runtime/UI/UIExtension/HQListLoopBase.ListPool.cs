using System.Collections;
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
                return null;
            }

            public void Recyle(HQListItem item)
            {
                
            }
        }
    }
}
