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
                private uint crc;
                private Type assetType;
                private bool inLoadingProgress;
                private Action<ResourceLoadCompleteEventArgs> onComplete;
                private Action<ResourceLoadErrorEventArgs> onError;

                public event Action<ResourceLoadCompleteEventArgs> LoadCompleteEvent
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

                public event Action<ResourceLoadErrorEventArgs> LoadErrorEvent
                {
                    add
                    {
                        onError += value;
                    }
                    remove
                    {
                        onError -= value;
                    }
                }

                public static ResourceLoadTask Create(ResourceManager resourceManager, uint crc, Type assetType, int priority, int groupID)
                {
                    ResourceLoadTask task = ReferencePool.Spawn<ResourceLoadTask>();
                    task.id = serialID++;
                    task.resourceManager = resourceManager;
                    task.crc = crc;
                    task.assetType = assetType;
                    task.priority = priority;
                    task.groupID = groupID;
                    return task;
                }

                public override TaskStartStatus Start()
                {
                    if (resourceManager.loadedAssetMap.ContainsKey(crc))
                    {
                        return TaskStartStatus.HasToWait;
                    }

                    return TaskStartStatus.InProgress;
                }

                public override void OnUpdate()
                {
                    if (inLoadingProgress)
                    {
                        return;
                    }
                    HQAssetItemConfig assetConfig = resourceManager.assetTable[crc];
                    bool ready = true;
                    for (int i = 0; i < assetConfig.dependencies.Length; i++)
                    {
                        // ready = ready && resourceManager[]
                    }
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceManager = null;
                    crc = 0;
                    assetType = null;
                    onComplete = null;
                    onError = null;
                    inLoadingProgress = false;
                }

                private void OnLoadAssetCompleteCallback(object asset)
                {
                    ResourceLoadCompleteEventArgs args = ResourceLoadCompleteEventArgs.Create(crc, asset);
                    resourceManager.loadedAssetMap[crc].assetObject = args.asset;
                    resourceManager.loadedAssetMap[crc].status = ResourceStatus.Ready;
                    onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);
                    status = TaskStatus.Done;
                }

                private void OnLoadAssetErrorCallback(string errorMessage)
                {
                    ResourceLoadErrorEventArgs args = ResourceLoadErrorEventArgs.Create(crc, resourceManager.assetTable[crc].assetPath, errorMessage);
                    resourceManager.loadedAssetMap[crc].status = ResourceStatus.Error;
                    onError?.Invoke(args);
                    ReferencePool.Recyle(args);
                    status = TaskStatus.Error;
                }
            }
        }
    }
}
