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
                if (!resourceManager.loadedBundleMap.ContainsKey(bundleID))
                {
                    BundleItem bundleItem = new BundleItem(bundleID);
                    resourceManager.loadedBundleMap.Add(bundleID, bundleItem);
                }
                
                BundleLoadTask task = BundleLoadTask.Create(resourceManager, bundleID, priority, groupID);
                int taskID = bundleLoadTaskDispatcher.AddTask(task);
            }

            public void OnUpdate()
            {
                bundleLoadTaskDispatcher.ProcessTasks();
            }
        }
    }
}
