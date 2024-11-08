using System;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class ResourceLoader
        {
            private ResourceManager resourceManager;
            private ResourceLoadTaskDispatcher resourceLoadTaskDispatcher;

            public ResourceLoader(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
                this.resourceLoadTaskDispatcher = new ResourceLoadTaskDispatcher(maxConcurrentLoadCount);
            }

            public void LoadAsset(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
            {
                if (!resourceManager.assetTable.ContainsKey(crc))
                {
                    ResourceLoadErrorEventArgs errorArgs = ResourceLoadErrorEventArgs.Create(crc, null, null, null, $"id : {crc} assets doesn't exist.");
                    onError?.Invoke(errorArgs);
                    ReferencePool.Recyle(errorArgs);
                    return;
                }

                HQAssetItemConfig assetInfo = resourceManager.assetTable[crc];
                ResourceLoadTask task = ResourceLoadTask.Create(resourceManager, assetInfo, assetType, priority, groupID);
                int taskID = resourceLoadTaskDispatcher.AddTask(task);
                resourceLoadTaskDispatcher.AddResourceLoadCompleteEvent(taskID, onComplete);
            }

            public void OnUpdate()
            {
                resourceLoadTaskDispatcher.ProcessTasks();
            }
        }
    }
}
