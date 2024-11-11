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
                    ResourceLoadErrorEventArgs args = ResourceLoadErrorEventArgs.Create(crc, null, $"id : {crc} asset doesn't exist.");
                    onError?.Invoke(args);
                    ReferencePool.Recyle(args);
                    return;
                }

                HQAssetItemConfig assetConfig = resourceManager.assetTable[crc];
                for (int i = 0; i < assetConfig.dependencies.Length; i++)
                {
                    LoadAsset(assetConfig.dependencies[i], null, null, null, priority, groupID);
                }

                resourceManager.bundleLoader.LoadBundle(assetConfig.bundleID, priority, groupID);
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
