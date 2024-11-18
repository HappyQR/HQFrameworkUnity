using UnityEngine;

namespace HQFramework.Runtime
{
    public class HQListLoopVertical
    {
        public enum LayoutMode : byte
        {
            TopToBottom,
            BottomToTop
        }

        [SerializeField]
        public uint numPerRow;
    }
}
