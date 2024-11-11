namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class ResourceLoader
        {
            private class ResourceLoadTaskDispatcher : ResumableTaskDispatcher<ResourceLoadTask>
            {
                public ResourceLoadTaskDispatcher(ushort maxConcurrentCount) : base(maxConcurrentCount)
                {
                }
            }
        }
    }
}
