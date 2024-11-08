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
                private HQAssetItemConfig assetItem;
                private Type assetType;

                private Action<ResourceLoadCompleteEventArgs> onComplete;

                public event Action<ResourceLoadCompleteEventArgs> OnComplete
                {
                    add { onComplete += value; }
                    remove { onComplete -= value; }
                }

                public static ResourceLoadTask Create(ResourceManager resourceManager, HQAssetItemConfig assetItem, Type assetType, int priority, int groupID)
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
                    // if (resourceManager.loadedBundleMap.ContainsKey(assetItem.bundleName))
                    // {
                    //     if (!resourceManager.loadedBundleMap[assetItem.bundleName].Ready)
                    //     {
                    //         return TaskStartStatus.HasToWait;
                    //     }
                    //     object bundleObject = resourceManager.loadedBundleMap[assetItem.bundleName].bundleObject;
                    //     resourceManager.resourceHelper.LoadAsset(bundleObject, assetItem.assetPath, assetType, OnLoadAssetCompleteCallback);
                    //     return TaskStartStatus.InProgress;
                    // }
                    // else
                    // {
                    //     HQAssetBundleConfig bundleInfo = resourceManager.localManifest.moduleDic[assetItem.moduleID].bundleDic[assetItem.bundleName];
                    //     resourceManager.bundleLoader.LoadBundle(bundleInfo, priority, groupID);
                    //     return TaskStartStatus.HasToWait;
                    // }
                    return TaskStartStatus.Done;
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
                    // ResourceLoadCompleteEventArgs args = ResourceLoadCompleteEventArgs.Create(assetItem.crc, asset);
                    // resourceManager.loadedAssetCrcMap.Add(args.asset, assetItem.crc);
                    // resourceManager.crcLoadedAssetMap.Add(assetItem.crc, args.asset);
                    // if (resourceManager.resourceHelper.IsIndividualAsset(args.asset))
                    // {
                    //     resourceManager.loadedBundleMap[assetItem.bundleName].refCount++;
                    // }
                    // onComplete?.Invoke(args);
                    // ReferencePool.Recyle(args);

                    // status = TaskStatus.Done;
                }
            }
        }
    }
}
