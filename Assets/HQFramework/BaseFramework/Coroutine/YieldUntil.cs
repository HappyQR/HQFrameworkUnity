using System;

namespace HQFramework.Coroutine
{
    public sealed class YieldUntil : IAsyncOperation
    {
        public bool isDone => predicate();
        
        private Func<bool> predicate;

        public YieldUntil(Func<bool> predicate)
        {
            this.predicate = predicate;
        }
    }
}
