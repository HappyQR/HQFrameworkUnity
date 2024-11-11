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
                private string bundleName;

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

                public static BundleLoadTask Create(ResourceManager resourceManager, string bundleName, int priority, int groupID)
                {
                    BundleLoadTask task = ReferencePool.Spawn<BundleLoadTask>();
                    task.id = serialID++;
                    task.priority = priority;
                    task.groupID = groupID;
                    task.resourceManager = resourceManager;
                    task.bundleName = bundleName;
                    return task;
                }

                public override TaskStartStatus Start()
                {
                    resourceManager.resourceHelper.LoadAssetBundle(resourceManager.GetBundleFilePath(resourceManager.bundleTable[bundleName]), OnLoadBundleComplete);
                    return TaskStartStatus.InProgress;
                }

                public override void OnUpdate()
                {
                    
                }

                private void OnLoadBundleComplete(object bundleObject)
                {
                    BundleLoadCompleteEventArgs args = BundleLoadCompleteEventArgs.Create(bundleName, bundleObject);
                    onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);

                    status = TaskStatus.Done;
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceManager = null;
                    bundleName = null;
                    onComplete = null;
                }
            }
        }
    }
}
