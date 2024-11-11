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

            public void LoadBundle(uint bundleID, Action<BundleLoadErrorEventArgs> onError, int priority, int groupID)
            {
                if (!resourceManager.bundleTable.ContainsKey(bundleID))
                {
                    BundleLoadErrorEventArgs args = BundleLoadErrorEventArgs.Create(resourceManager.bundleTable[bundleID].bundleName, $"{resourceManager.bundleTable[bundleID].bundleName} doesn't exist.");
                    onError?.Invoke(args);
                    ReferencePool.Recyle(args);
                    return;
                }

                if (resourceManager.loadedBundleMap.ContainsKey(bundleID))
                {
                    return;
                }
                resourceManager.loadedBundleMap.Add(bundleID, new BundleItem(resourceManager.bundleTable[bundleID], null));
                BundleLoadTask task = BundleLoadTask.Create(resourceManager, bundleID, priority, groupID);
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
                resourceManager.loadedBundleMap[args.bundleID].bundleObject = args.bundleObject;
            }
        }
    }
}
