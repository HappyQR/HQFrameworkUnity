using System;
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
                private uint crc;
                private Type assetType;
                private Queue<uint> dependenceQueue;

                private Action<ResourceLoadCompleteEventArgs> onComplete;
                private Action<ResourceLoadErrorEventArgs> onError;

                public event Action<ResourceLoadCompleteEventArgs> OnComplete
                {
                    add { onComplete += value; }
                    remove { onComplete -= value; }
                }

                public event Action<ResourceLoadErrorEventArgs> OnError
                {
                    add { onError += value; }
                    remove { onError -= value; }
                }

                public static ResourceLoadTask Create(ResourceManager resourceManager, uint crc, Type assetType, int priority, int groupID)
                {
                    ResourceLoadTask task = ReferencePool.Spawn<ResourceLoadTask>();
                    task.id = serialID++;
                    task.priority = priority;
                    task.groupID = groupID;
                    task.crc = crc;
                    task.assetType = assetType;
                    task.resourceManager = resourceManager;
                    if (task.dependenceQueue == null)
                    {
                        task.dependenceQueue = new Queue<uint>();
                    }
                    for (int i = 0; i < resourceManager.assetTable[crc].dependencies.Length; i++)
                    {
                        task.dependenceQueue.Enqueue(resourceManager.assetTable[crc].dependencies[i]);
                    }
                    return task;
                }

                public override TaskStartStatus Start()
                {
                    if (!resourceManager.assetTable.ContainsKey(crc))
                    {
                        ResourceLoadErrorEventArgs errorArgs = ResourceLoadErrorEventArgs.Create(crc, null, null, null, $"id : {crc} assets doesn't exist.");
                        onError?.Invoke(errorArgs);
                        ReferencePool.Recyle(errorArgs);
                        return TaskStartStatus.Error;
                    }

                    HQAssetItemConfig assetConfig = resourceManager.assetTable[crc];
                    if (resourceManager.loadedAssetMap.ContainsKey(crc))
                    {
                        if (resourceManager.loadedAssetMap[crc].Ready)
                        {
                            ResourceLoadCompleteEventArgs args = ResourceLoadCompleteEventArgs.Create(crc, resourceManager.loadedAssetMap[crc]);
                            onComplete?.Invoke(args);
                            ReferencePool.Recyle(args);
                            return TaskStartStatus.Done;
                        }
                        else if (resourceManager.loadedAssetMap[crc].error)
                        {
                            
                            return TaskStartStatus.Error;
                        }
                    }

                    resourceManager.loadedAssetMap.Add(crc, new AssetItem(assetConfig, null));
                    int loopCount = dependenceQueue.Count;
                    for (int i = 0; i < loopCount; i++)
                    {
                        uint dependenceCrc = dependenceQueue.Dequeue();
                        if (resourceManager.loadedAssetMap.ContainsKey(dependenceCrc))
                        {
                            AssetItem item = resourceManager.loadedAssetMap[dependenceCrc];
                            if (!item.Ready)
                            {
                                dependenceQueue.Enqueue(dependenceCrc);
                            }
                        }
                        else
                        {
                            HQAssetItemConfig dependenceAssetConfig = resourceManager.assetTable[dependenceCrc];
                            resourceManager.LoadAsset(dependenceAssetConfig.crc, null, onError, priority, groupID);
                            dependenceQueue.Enqueue(dependenceCrc);
                        }
                    }

                    if (!resourceManager.loadedBundleMap.ContainsKey(assetConfig.bundleName))
                    {
                        resourceManager.bundleLoader.LoadBundle(assetConfig.bundleName, priority, groupID);
                    }

                    if (dependenceQueue.Count > 0 || !resourceManager.loadedBundleMap.TryGetValue(assetConfig.bundleName, out BundleItem bundle) || !bundle.Ready)
                    {
                        return TaskStartStatus.HasToWait;
                    }

                    resourceManager.resourceHelper.LoadAsset(resourceManager.loadedBundleMap[assetConfig.bundleName].bundleObject, assetConfig.assetPath, OnLoadAssetCompleteCallback);
                    return TaskStartStatus.InProgress;
                }

                public override void OnUpdate()
                {
                    
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceManager = null;
                    crc = 0;
                    assetType = null;
                    onComplete = null;
                    onError = null;
                    dependenceQueue.Clear();
                }

                private void OnLoadAssetCompleteCallback(object asset)
                {
                    ResourceLoadCompleteEventArgs args = ResourceLoadCompleteEventArgs.Create(crc, asset);
                    onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);
                    status = TaskStatus.Done;
                }

                private void OnLoadDependenceError(ResourceLoadErrorEventArgs args)
                {
                    HQAssetItemConfig assetConfig = resourceManager.assetTable[crc];
                    ResourceLoadErrorEventArgs errorArgs = ResourceLoadErrorEventArgs.Create(crc, assetConfig.assetPath, assetConfig.bundleName, assetConfig.bundleName, $"id : {crc} asset load failed. {args.errorMessage}");
                    onError?.Invoke(errorArgs);
                    ReferencePool.Recyle(errorArgs);
                }

                private void OnLoadBundleError(BundleLoadErrorEventArgs args)
                {
                    HQAssetItemConfig assetConfig = resourceManager.assetTable[crc];
                    ResourceLoadErrorEventArgs errorArgs = ResourceLoadErrorEventArgs.Create(crc, assetConfig.assetPath, assetConfig.bundleName, assetConfig.bundleName, $"id : {crc} asset load failed. {args.errorMessage}");
                    onError?.Invoke(errorArgs);
                    ReferencePool.Recyle(errorArgs);
                }
            }
        }
    }
}
