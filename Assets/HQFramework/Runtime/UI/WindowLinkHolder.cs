using System.Collections.Generic;
using UnityEngine;

namespace HQFramework.Runtime
{
    public sealed class WindowLinkHolder : MonoBehaviour
    {
        [SerializeField]
        internal bool destroyOnHide;

        [SerializeField]
        internal List<RectTransform> linkedElementList;
    }
}
