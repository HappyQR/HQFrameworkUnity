using System;

namespace HQFramework.Coroutine
{
    public sealed class YieldWhile : IAsyncOperation
    {
        public bool isDone => !predicate();

        private Func<bool> predicate;

        public YieldWhile(Func<bool> predicate)
        {
            this.predicate = predicate;
        }
    }
}
