using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQFramework.Runtime
{
    public abstract partial class HQListLoopBase : HQListBase
    {
        [SerializeField]
        protected HQScrollRect scrollRect;

        protected ListPool listPool;

        protected override void Awake()
        {
            base.Awake();
            listPool = new ListPool(this);
            scrollRect.ScrollEvent.AddListener(CalculateListPosition);
        }

        public override void SetItemCount(int count)
        {
            
        }

        protected abstract void CalculateListPosition(Vector2 delta);
    }
}
