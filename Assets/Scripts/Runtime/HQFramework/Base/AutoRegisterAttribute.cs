using System;

namespace HQFramework
{
    /// <summary>
    /// Module will be auto registered by HQFrameworkEngine with this attribute, internal class.
    /// </summary>
    internal class AutoRegisterAttribute : Attribute
    {
        public byte groupOrder;
        public byte internalOrder;

        public AutoRegisterAttribute(byte groupOrder, byte internalOrder = byte.MaxValue)
        {
            this.groupOrder = groupOrder;
            this.internalOrder = internalOrder;
        }
    }
}
