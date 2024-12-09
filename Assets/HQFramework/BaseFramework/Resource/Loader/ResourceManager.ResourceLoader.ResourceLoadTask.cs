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
                    if (!resourceManager.assetTable.ContainsKey(crc))
                    {
                        ResourceLoadErrorEventArgs args = ResourceLoadErrorEventArgs.Create(crc, null, $"id : {crc} asset doesn't exist.");
                        onError?.Invoke(args);
                        ReferencePool.Recyle(args);
                        resourceManager.loadedAssetMap[crc].status = ResourceStatus.Error;
                        return TaskStartStatus.Error;
                    }

                    HQAssetItemConfig assetConfig = resourceManager.assetTable[crc];
                    AssetItem assetItem = resourceManager.loadedAssetMap[crc];
                    switch (assetItem.status)
                    {
                        case ResourceStatus.Ready:
                            ResourceLoadCompleteEventArgs completeEventArgs = ResourceLoadCompleteEventArgs.Create(crc, assetItem.assetObject);
                            onComplete?.Invoke(completeEventArgs);
                            AssetPendingItem pendingItem = resourceManager.pendingAssetDic[crc];
                            pendingItem.count--;
                            if (pendingItem.count == 0)
                            {
                                ReferencePool.Recyle(pendingItem);
                                resourceManager.pendingAssetDic.Remove(crc);
                            }
                            ReferencePool.Recyle(completeEventArgs);
                            return TaskStartStatus.Done;
                            
                        case ResourceStatus.Error:
                            ResourceLoadErrorEventArgs errorEventArgs = ResourceLoadErrorEventArgs.Create(crc, assetConfig.assetPath, $"id : {crc} asset load failed.");
                            onError?.Invoke(errorEventArgs);
                            ReferencePool.Recyle(errorEventArgs);
                            return TaskStartStatus.Error;
                            
                        case ResourceStatus.InProgress:
                            return TaskStartStatus.HasToWait;
                            
                        default:
                            for (int i = 0; i < assetConfig.dependencies.Length; i++)
                            {
                                resourceManager.resourceLoader.LoadAsset(assetConfig.dependencies[i], null, null, null, priority, groupID);
                            }
                            resourceManager.bundleLoader.LoadBundle(assetConfig.bundleID, priority, groupID);
                            assetItem.status = ResourceStatus.InProgress;
                            status = TaskStatus.InProgress;
                            return TaskStartStatus.InProgress;
                    }
                }

                public override void OnUpdate()
                {
                    if (inLoadingProgress)
                        return;

                    HQAssetItemConfig assetConfig = resourceManager.assetTable[crc];
                    bool ready = true, error = false;
                    for (int i = 0; i < assetConfig.dependencies.Length; i++)
                    {
                        ResourceStatus dependenceStatus = resourceManager.loadedAssetMap[assetConfig.dependencies[i]].status;
                        ready = ready && dependenceStatus == ResourceStatus.Ready;
                        error = error || dependenceStatus == ResourceStatus.Error;
                    }
                    ResourceStatus bundleStatus = resourceManager.loadedBundleMap[assetConfig.bundleID].status;
                    ready = ready && bundleStatus == ResourceStatus.Ready;
                    error = error || bundleStatus == ResourceStatus.Error;

                    if (ready)
                    {
                        if (assetType != null)
                        {
                            resourceManager.resourceHelper.LoadAsset(resourceManager.loadedBundleMap[assetConfig.bundleID].bundleObject, assetConfig.assetPath, assetType, OnLoadAssetCompleteCallback, OnLoadAssetErrorCallback);
                        }
                        else
                        {
                            resourceManager.resourceHelper.LoadAsset(resourceManager.loadedBundleMap[assetConfig.bundleID].bundleObject, assetConfig.assetPath, OnLoadAssetCompleteCallback, OnLoadAssetErrorCallback);
                        }
                        inLoadingProgress = true;
                    }
                    else if (error)
                    {
                        ResourceLoadErrorEventArgs errorEventArgs = ResourceLoadErrorEventArgs.Create(crc, assetConfig.assetPath, $"id : {crc} asset load failed.");
                        onError?.Invoke(errorEventArgs);
                        if (resourceManager.pendingAssetDic.TryGetValue(crc, out AssetPendingItem pendingItem))
                        {
                            ReferencePool.Recyle(pendingItem);
                            resourceManager.pendingAssetDic.Remove(crc);
                        }
                        ReferencePool.Recyle(errorEventArgs);
                        resourceManager.loadedAssetMap[crc].status = ResourceStatus.Error;
                        status = TaskStatus.Error;
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
                    HQAssetItemConfig assetConfig = resourceManager.assetTable[crc];
                    ResourceLoadCompleteEventArgs args = ResourceLoadCompleteEventArgs.Create(crc, asset);
                    resourceManager.loadedAssetMap[crc].assetObject = args.asset;
                    resourceManager.loadedAssetMap[crc].status = ResourceStatus.Ready;
                    AssetPendingItem pendingItem = resourceManager.pendingAssetDic[crc];
                    pendingItem.count--;
                    if (pendingItem.count == 0)
                    {
                        ReferencePool.Recyle(pendingItem);
                        resourceManager.pendingAssetDic.Remove(crc);
                    }
                    onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);
                    status = TaskStatus.Done;
                }

                private void OnLoadAssetErrorCallback(string errorMessage)
                {
                    ResourceLoadErrorEventArgs args = ResourceLoadErrorEventArgs.Create(crc, resourceManager.assetTable[crc].assetPath, errorMessage);
                    resourceManager.loadedAssetMap[crc].status = ResourceStatus.Error;
                    if (resourceManager.pendingAssetDic.TryGetValue(crc, out AssetPendingItem pendingItem))
                    {
                        ReferencePool.Recyle(pendingItem);
                        resourceManager.pendingAssetDic.Remove(crc);
                    }
                    onError?.Invoke(args);
                    ReferencePool.Recyle(args);
                    status = TaskStatus.Error;
                }
            }
        }
    }
}
