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
                private ResourceLoader resourceLoader;
                private IResourceHelper resourceHelper;
                private AssetItemInfo assetItem;
                private Type assetType;

                private Action<ResourceLoadCompleteEventArgs> onComplete;

                public event Action<ResourceLoadCompleteEventArgs> OnComplete
                {
                    add { onComplete += value; }
                    remove { onComplete -= value; }
                }

                public static ResourceLoadTask Create(ResourceLoader resourceLoader, IResourceHelper resourceHelper, AssetItemInfo assetItem, Type assetType, int priority, int groupID)
                {
                    ResourceLoadTask task = ReferencePool.Spawn<ResourceLoadTask>();
                    task.id = serialID++;
                    task.priority = priority;
                    task.groupID = groupID;
                    task.assetItem = assetItem;
                    task.assetType = assetType;
                    task.resourceLoader = resourceLoader;
                    task.resourceHelper = resourceHelper;
                    return task;
                }

                public override TaskStartStatus Start()
                {
                    if (resourceLoader.loadedBundleDic.ContainsKey(assetItem.bundleName))
                    {
                        if (resourceLoader.loadedBundleDic[assetItem.bundleName] == null)
                        {
                            return TaskStartStatus.HasToWait;
                        }
                        else
                        {
                            resourceHelper.LoadAsset(resourceLoader.loadedBundleDic[assetItem.bundleName], assetItem.assetPath, assetType, OnLoadAssetCompleteCallback);
                            return TaskStartStatus.InProgress;
                        }
                    }
                    else
                    {
                        AssetBundleInfo bundleInfo = resourceLoader.resourceManager.localManifest.moduleDic[assetItem.moduleID].bundleDic[assetItem.bundleName];
                        resourceLoader.LoadBundle(bundleInfo, priority, groupID);
                        return TaskStartStatus.HasToWait;
                    }
                }

                public override void OnUpdate()
                {
                    
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceLoader = null;
                    resourceHelper = null;
                    assetItem = null;
                    assetType = null;
                    onComplete = null;
                }

                private void OnLoadAssetCompleteCallback(object asset)
                {
                    ResourceLoadCompleteEventArgs args = ResourceLoadCompleteEventArgs.Create(assetItem.crc, asset);
                    onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);

                    status = TaskStatus.Done;
                }
            }
        }
    }
}
