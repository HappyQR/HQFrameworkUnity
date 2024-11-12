namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class BundleLoader
        {
            private class BundleLoadTaskDispatcher : ResumableTaskDispatcher<BundleLoadTask>
            {
                public BundleLoadTaskDispatcher(ushort maxConcurrentCount) : base(maxConcurrentCount)
                {
                }
            }
        }
    }
}
