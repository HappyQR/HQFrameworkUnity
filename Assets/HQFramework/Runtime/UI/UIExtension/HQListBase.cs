using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HQFramework.Runtime
{
    public abstract class HQListBase : UIBehaviour
    {
        public abstract void SetItemCount(int count);
    }
}
