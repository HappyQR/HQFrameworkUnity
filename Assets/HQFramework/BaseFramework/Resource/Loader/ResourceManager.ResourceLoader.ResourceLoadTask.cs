using System;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class ResourceLoader
        {
            private class ResourceLoadTask : ResumableTask
            {
                private static int serialID = 0;
                private ResourceManager resourceManager;
                private AssetItemInfo assetItem;
                private Type assetType;

                private Action<ResourceLoadCompleteEventArgs> onComplete;

                public event Action<ResourceLoadCompleteEventArgs> OnComplete
                {
                    add { onComplete += value; }
                    remove { onComplete -= value; }
                }

                public static ResourceLoadTask Create(ResourceManager resourceManager, AssetItemInfo assetItem, Type assetType, int priority, int groupID)
                {
                    ResourceLoadTask task = ReferencePool.Spawn<ResourceLoadTask>();
                    task.id = serialID++;
                    task.priority = priority;
                    task.groupID = groupID;
                    task.assetItem = assetItem;
                    task.assetType = assetType;
                    task.resourceManager = resourceManager;
                    return task;
                }

                public override TaskStartStatus Start()
                {
                    return TaskStartStatus.HasToWait;
                }

                public override void OnUpdate()
                {
                    
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceManager = null;
                    assetItem = null;
                    assetType = null;
                    onComplete = null;
                }

                private void OnLoadAssetCompleteCallback(object asset)
                {
                    ResourceLoadCompleteEventArgs args = ResourceLoadCompleteEventArgs.Create(assetItem.crc, asset);
                    onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);

                    status = TaskStatus.Done;
                }
            }
        }
    }
}