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

            public void LoadBundle(string bundleName, int priority, int groupID)
            {
                BundleLoadTask task = BundleLoadTask.Create(resourceManager, bundleName, priority, groupID);
                int taskID = bundleLoadTaskDispatcher.AddTask(task);
                bundleLoadTaskDispatcher.AddBundleLoadCompleteCallback(taskID, OnLoadBundleComplete);
            }

            public void OnUpdate()
            {
                bundleLoadTaskDispatcher.ProcessTasks();
            }

            private void OnLoadBundleComplete(BundleLoadCompleteEventArgs args)
            {
                // resourceManager.loadingBundleSet.Remove(resourceManager.loadedBundleMap[args.bundleName]);
                // resourceManager.loadedBundleMap[args.bundleName].bundleObject = args.bundleObject;
            }
        }
    }
}
