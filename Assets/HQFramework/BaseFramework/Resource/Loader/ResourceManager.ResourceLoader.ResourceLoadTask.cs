using System.Collections.Generic;

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
                private ResourceLoadTaskInfo info;
                private Queue<uint> dependenceQueue;

                public static ResourceLoadTask Create(ResourceManager resourceManager, ResourceLoadTaskInfo info)
                {
                    ResourceLoadTask task = ReferencePool.Spawn<ResourceLoadTask>();
                    task.id = serialID++;
                    task.info = info;
                    task.resourceManager = resourceManager;
                    if (task.dependenceQueue == null)
                    {
                        task.dependenceQueue = new Queue<uint>();
                    }
                    for (int i = 0; i < resourceManager.assetTable[info.crc].dependencies.Length; i++)
                    {
                        task.dependenceQueue.Enqueue(resourceManager.assetTable[info.crc].dependencies[i]);
                    }
                    return task;
                }

                public override TaskStartStatus Start()
                {
                    HQAssetItemConfig assetConfig = resourceManager.assetTable[info.crc];
                    int loopCount = dependenceQueue.Count;
                    for (int i = 0; i < loopCount; i++)
                    {
                        uint dependenceCrc = dependenceQueue.Dequeue();
                        if (resourceManager.loadedAssetMap.ContainsKey(dependenceCrc))
                        {
                            AssetItem item = resourceManager.loadedAssetMap[dependenceCrc];
                            if (item.error)
                            {
                                return TaskStartStatus.Error;
                            }
                            else if (!item.Ready)
                            {
                                dependenceQueue.Enqueue(dependenceCrc);
                            }
                        }
                        else
                        {
                            HQAssetItemConfig dependenceAssetConfig = resourceManager.assetTable[dependenceCrc];
                            resourceManager.LoadAsset(dependenceAssetConfig.crc, null, null, OnLoadDependenceError, priority, groupID);
                            dependenceQueue.Enqueue(dependenceCrc);
                        }
                    }

                    if (!resourceManager.loadedBundleMap.ContainsKey(assetConfig.crc))
                    {
                        resourceManager.bundleLoader.LoadBundle(assetConfig.bundleID, OnLoadBundleError, priority, groupID);
                    }
                    else if (resourceManager.loadedBundleMap[assetConfig.bundleID].error)
                    {
                        return TaskStartStatus.Error;
                    }

                    if (dependenceQueue.Count > 0 || !resourceManager.loadedBundleMap[assetConfig.bundleID].Ready)
                    {
                        return TaskStartStatus.HasToWait;
                    }

                    if (info.assetType == null)
                    {
                        resourceManager.resourceHelper.LoadAsset(resourceManager.loadedBundleMap[assetConfig.bundleID].bundleObject, assetConfig.assetPath, OnLoadAssetCompleteCallback);                        
                    }
                    else
                    {
                        resourceManager.resourceHelper.LoadAsset(resourceManager.loadedBundleMap[assetConfig.bundleID].bundleObject, assetConfig.assetPath, info.assetType, OnLoadAssetCompleteCallback);
                    }
                    return TaskStartStatus.InProgress;
                }

                public override void OnUpdate()
                {

                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    ReferencePool.Recyle(info);
                    resourceManager = null;
                    dependenceQueue.Clear();
                }

                private void OnLoadAssetCompleteCallback(object asset)
                {
                    ResourceLoadCompleteEventArgs args = ResourceLoadCompleteEventArgs.Create(info.crc, asset);
                    resourceManager.loadedAssetMap[info.crc].assetObject = args.asset;
                    info.onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);
                    status = TaskStatus.Done;
                }

                private void OnLoadDependenceError(ResourceLoadErrorEventArgs args)
                {
                    HQAssetItemConfig assetConfig = resourceManager.assetTable[info.crc];
                    ResourceLoadErrorEventArgs errorArgs = ResourceLoadErrorEventArgs.Create(info.crc, assetConfig.assetPath, assetConfig.bundleName, assetConfig.moduleID, $"id : {info.crc} asset load failed. {args.errorMessage}");
                    info.onError?.Invoke(errorArgs);
                    ReferencePool.Recyle(errorArgs);
                    resourceManager.loadedAssetMap[info.crc].error = true;
                    status = TaskStatus.Error;
                }

                private void OnLoadBundleError(BundleLoadErrorEventArgs args)
                {
                    HQAssetItemConfig assetConfig = resourceManager.assetTable[info.crc];
                    ResourceLoadErrorEventArgs errorArgs = ResourceLoadErrorEventArgs.Create(info.crc, assetConfig.assetPath, assetConfig.bundleName, assetConfig.moduleID, $"id : {info.crc} asset load failed. {args.errorMessage}");
                    info.onError?.Invoke(errorArgs);
                    ReferencePool.Recyle(errorArgs);
                    resourceManager.loadedAssetMap[info.crc].error = true;
                    status = TaskStatus.Error;
                }
            }
        }
    }
}
