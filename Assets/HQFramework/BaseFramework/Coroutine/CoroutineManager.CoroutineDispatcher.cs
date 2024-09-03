namespace HQFramework.Coroutine
{
    internal partial class CoroutineManager
    {
        private sealed class CoroutineDispatcher : ResumableTaskDispatcher<CoroutineTask>
        {
            public CoroutineDispatcher(ushort maxConcurrentCount) : base(maxConcurrentCount)
            {
            }
        }
    }
}
