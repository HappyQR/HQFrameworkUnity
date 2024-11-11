using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class ResourceLoader
        {
            private ResourceManager resourceManager;
            private ResourceLoadTaskDispatcher resourceLoadTaskDispatcher;
            private Queue<ResourceLoadTaskInfo> inProgressQueue;

            public ResourceLoader(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
                this.resourceLoadTaskDispatcher = new ResourceLoadTaskDispatcher(maxConcurrentLoadCount);
                this.inProgressQueue = new Queue<ResourceLoadTaskInfo>();
            }

            public void LoadAsset(ResourceLoadTaskInfo info)
            {
                if (!resourceManager.assetTable.ContainsKey(info.crc))
                {
                    ResourceLoadErrorEventArgs errorArgs = ResourceLoadErrorEventArgs.Create(info.crc, null, null, -1, $"id : {info.crc} assets doesn't exist.");
                    info.onError?.Invoke(errorArgs);
                    ReferencePool.Recyle(errorArgs);
                    ReferencePool.Recyle(info);
                    return;
                }

                if (resourceManager.loadedAssetMap.ContainsKey(info.crc))
                {
                    if (resourceManager.loadedAssetMap[info.crc].Ready)
                    {
                        ResourceLoadCompleteEventArgs args = ResourceLoadCompleteEventArgs.Create(info.crc, resourceManager.loadedAssetMap[info.crc].assetObject);
                        info.onComplete?.Invoke(args);
                        ReferencePool.Recyle(args);
                        ReferencePool.Recyle(info);
                    }
                    else
                    {
                        inProgressQueue.Enqueue(info);
                    }
                    return;
                }

                HQAssetItemConfig assetConfig = resourceManager.assetTable[info.crc];
                resourceManager.loadedAssetMap.Add(info.crc, new AssetItem(assetConfig, null));
                ResourceLoadTask task = ResourceLoadTask.Create(resourceManager, info);
                resourceLoadTaskDispatcher.AddTask(task);
            }

            public void OnUpdate()
            {
                resourceLoadTaskDispatcher.ProcessTasks();

                int loopCount = inProgressQueue.Count;
                for (int i = 0; i < loopCount; i++)
                {
                    ResourceLoadTaskInfo info = inProgressQueue.Dequeue();
                    if (resourceManager.loadedAssetMap[info.crc].Ready)
                    {
                        ResourceLoadCompleteEventArgs args = ResourceLoadCompleteEventArgs.Create(info.crc, resourceManager.loadedAssetMap[info.crc].assetObject);
                        info.onComplete?.Invoke(args);
                        ReferencePool.Recyle(args);
                        ReferencePool.Recyle(info);
                    }
                    else if (resourceManager.loadedAssetMap[info.crc].error)
                    {
                        HQAssetItemConfig assetConfig = resourceManager.assetTable[info.crc];
                        ResourceLoadErrorEventArgs args = ResourceLoadErrorEventArgs.Create(assetConfig.crc, assetConfig.assetPath, assetConfig.bundleName, assetConfig.moduleID, "Asset Load Failed.");
                        info.onError?.Invoke(args);
                        ReferencePool.Recyle(args);
                        ReferencePool.Recyle(info);
                    }
                    else
                    {
                        inProgressQueue.Enqueue(info);
                    }
                }
            }
        }
    }
}
