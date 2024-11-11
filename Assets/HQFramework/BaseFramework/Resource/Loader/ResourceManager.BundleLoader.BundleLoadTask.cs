using System;

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

                private Action<BundleLoadCompleteEventArgs> onComplete;

                public event Action<BundleLoadCompleteEventArgs> OnComplete
                {
                    add
                    {
                        onComplete += value;
                    }
                    remove
                    {
                        onComplete -= value;
                    }
                }

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
                    return TaskStartStatus.InProgress;
                }

                public override void OnUpdate()
                {
                    
                }

                private void OnLoadBundleComplete(object bundleObject)
                {
                    BundleLoadCompleteEventArgs args = BundleLoadCompleteEventArgs.Create(bundleID, resourceManager.bundleTable[bundleID].bundleName, bundleObject);
                    onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);

                    status = TaskStatus.Done;
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceManager = null;
                    bundleID = 0;
                    onComplete = null;
                }
            }
        }
    }
}
