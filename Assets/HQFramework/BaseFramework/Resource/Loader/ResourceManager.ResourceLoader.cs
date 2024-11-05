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
                if (!resourceManager.assetItemMap.ContainsKey(crc))
                {
                    ResourceLoadErrorEventArgs errorArgs = ResourceLoadErrorEventArgs.Create(crc, null, null, null, $"id : {crc} assets doesn't exist.");
                    onError?.Invoke(errorArgs);
                    ReferencePool.Recyle(errorArgs);
                    return;
                }

                AssetItemInfo assetInfo = resourceManager.assetItemMap[crc];
                ResourceLoadTask task = ResourceLoadTask.Create(resourceManager, assetInfo, assetType, priority, groupID);
                int taskID = resourceLoadTaskDispatcher.AddTask(task);
                resourceLoadTaskDispatcher.AddResourceLoadCompleteEvent(taskID, onComplete);
            }

            public void LoadAsset<T>(uint crc, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class
            {
                void OnLoadComplete(ResourceLoadCompleteEventArgs originalArgs)
                {
                    ResourceLoadCompleteEventArgs<T> args = ResourceLoadCompleteEventArgs<T>.Create(originalArgs.crc, (T)originalArgs.asset);
                    onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);
                }

                Type assetType = typeof(T);
                LoadAsset(crc, assetType, OnLoadComplete, onError, priority, groupID);
            }

            public void OnUpdate()
            {
                resourceLoadTaskDispatcher.ProcessTasks();
            }
        }
    }
}
