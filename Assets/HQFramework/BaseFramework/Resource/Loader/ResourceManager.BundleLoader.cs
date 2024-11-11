using System;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class BundleLoader
        {
            private ResourceManager resourceManager;
            private BundleLoadTaskDispatcher bundleLoadTaskDispatcher;

            public BundleLoader(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
                this.bundleLoadTaskDispatcher = new BundleLoadTaskDispatcher(maxConcurrentLoadCount);
            }

            public void LoadBundle(uint bundleID, int priority, int groupID)
            {
                
            }

            public void OnUpdate()
            {
                bundleLoadTaskDispatcher.ProcessTasks();
            }
        }
    }
}
