namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class BundleLoader
        {
            private class BundleLoadTask : ResumableTask
            {
                private static int serialID = 0;

                private ResourceManager resourceManager;
                private uint bundleID;

                public static BundleLoadTask Create(ResourceManager resourceManager, uint bundleID, int priority, int groupID)
                {
                    BundleLoadTask task = ReferencePool.Spawn<BundleLoadTask>();
                    task.id = serialID++;
                    task.priority = priority;
                    task.groupID = groupID;
                    task.resourceManager = resourceManager;
                    task.bundleID = bundleID;
                    return task;
                }

                public override TaskStartStatus Start()
                {
                    if (!resourceManager.bundleTable.ContainsKey(bundleID))
                    {
                        resourceManager.loadedBundleMap[bundleID].status = ResourceStatus.Error;
                        return TaskStartStatus.Error;
                    }

                    HQAssetBundleConfig bundleConfig = resourceManager.bundleTable[bundleID];
                    BundleItem bundleItem = resourceManager.loadedBundleMap[bundleID];
                    switch (bundleItem.status)
                    {
                        case ResourceStatus.Ready:
                            return TaskStartStatus.Done;

                        case ResourceStatus.Error:
                            return TaskStartStatus.Error;

                        case ResourceStatus.InProgress:
                            return TaskStartStatus.HasToWait;

                        default:
                            string bundlePath = resourceManager.GetBundleFilePath(bundleConfig);
                            resourceManager.resourceHelper.LoadAssetBundle(bundlePath, OnLoadBundleCompleteCallback, OnLoadBundleErrorCallback);
                            bundleItem.status = ResourceStatus.InProgress;
                            status = TaskStatus.InProgress;
                            return TaskStartStatus.InProgress;
                    }
                }

                public override void OnUpdate()
                {
                    
                }

                private void OnLoadBundleCompleteCallback(object bundleObject)
                {
                    resourceManager.loadedBundleMap[bundleID].bundleObject = bundleObject;
                    resourceManager.loadedBundleMap[bundleID].status = ResourceStatus.Ready;
                    status = TaskStatus.Done;
                }

                private void OnLoadBundleErrorCallback(string errorMessage)
                {
                    resourceManager.loadedBundleMap[bundleID].status = ResourceStatus.Error;
                    status = TaskStatus.Error;
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceManager = null;
                    bundleID = 0;
                }
            }
        }
    }
}
