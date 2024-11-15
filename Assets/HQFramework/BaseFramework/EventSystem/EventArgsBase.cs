using System;

namespace HQFramework.EventSystem
{
    public abstract class EventArgsBase : EventArgs, IReference
    {
        public abstract int SerialID { get; }

        protected abstract void OnRecyle();

        void IReference.OnRecyle()
        {
            OnRecyle();
        }
    }
}
