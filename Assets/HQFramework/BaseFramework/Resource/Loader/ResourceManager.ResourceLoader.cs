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
                if (!resourceManager.loadedAssetMap.ContainsKey(crc))
                {
                    AssetItem assetItem = new AssetItem(crc);
                    resourceManager.loadedAssetMap.Add(crc, assetItem);
                }

                if (resourceManager.assetTable.TryGetValue(crc, out HQAssetItemConfig assetConfig))
                {
                    if (!resourceManager.pendingAssetDic.ContainsKey(crc))
                    {
                        resourceManager.pendingAssetDic.Add(crc, AssetPendingItem.Create(assetConfig));
                    }
                    resourceManager.pendingAssetDic[crc].count++;
                }
                
                ResourceLoadTask task = ResourceLoadTask.Create(resourceManager, crc, assetType, priority, groupID);
                int taskID = resourceLoadTaskDispatcher.AddTask(task);
                resourceLoadTaskDispatcher.AddResourceLoadCompleteEvent(taskID, onComplete);
                resourceLoadTaskDispatcher.AddResourceLoadErrorEvent(taskID, onError); 
            }

            public void OnUpdate()
            {
                resourceLoadTaskDispatcher.ProcessTasks();
            }
        }
    }
}
