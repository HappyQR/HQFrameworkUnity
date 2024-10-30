using System;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class ResourceLoader
        {
            private class BundleLoadTaskDispatcher : ResumableTaskDispatcher<BundleLoadTask>
            {
                public BundleLoadTaskDispatcher(ushort maxConcurrentCount) : base(maxConcurrentCount)
                {
                }

                public void AddResourceLoadErrorEvent(int taskID, Action<ResourceLoadErrorEventArgs> onError)
                {

                }
            }
        }
    }
}
