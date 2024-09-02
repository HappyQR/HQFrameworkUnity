using System.Collections;

namespace HQFramework.Coroutine
{
    internal partial class CoroutineManager
    {
        private class CoroutineDispatcher : ResumableTaskDispatcher<CoroutineTask>
        {
            public CoroutineDispatcher(int maxConcurrentCount) : base(maxConcurrentCount)
            {
            }
        }
    }
}
