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

            public void LoadBundle(string bundleName, Action<BundleLoadErrorEventArgs> onError, int priority, int groupID)
            {
                if (!resourceManager.bundleTable.ContainsKey(bundleName))
                {
                    BundleLoadErrorEventArgs args = BundleLoadErrorEventArgs.Create(bundleName, $"{bundleName} doesn't exist.");
                    onError?.Invoke(args);
                    ReferencePool.Recyle(args);
                    return;
                }

                if (resourceManager.loadedBundleMap.ContainsKey(bundleName))
                {
                    return;
                }
                resourceManager.loadedBundleMap.Add(bundleName, new BundleItem(resourceManager.bundleTable[bundleName], null));
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
                resourceManager.loadedBundleMap[args.bundleName].bundleObject = args.bundleObject;
            }
        }
    }
}
