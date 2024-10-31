using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class ResourceLoader
        {
            private ResourceManager resourceManager;
            private ResourceLoadTaskDispatcher resourceLoadTaskDispatcher;
            private BundleLoadTaskDispatcher bundleLoadTaskDispatcher;

            private Dictionary<uint, AssetItemInfo> assetItemDic;
            private Dictionary<string, object> loadedBundleDic;
            private Dictionary<string, int> bundleReferenceCountDic;

            private static readonly ushort maxConcurrentLoadCount = 1024;

            public ResourceLoader(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
                this.resourceLoadTaskDispatcher = new ResourceLoadTaskDispatcher(maxConcurrentLoadCount);
                this.bundleLoadTaskDispatcher = new BundleLoadTaskDispatcher(maxConcurrentLoadCount);
                this.assetItemDic = new Dictionary<uint, AssetItemInfo>();
                this.loadedBundleDic = new Dictionary<string, object>();
                this.bundleReferenceCountDic = new Dictionary<string, int>();
            }

            public void LoadAsset(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
            {
                if (!assetItemDic.ContainsKey(crc))
                {
                    ResourceLoadErrorEventArgs errorArgs = ResourceLoadErrorEventArgs.Create(crc, null, null, null, $"id : {crc} assets doesn't exist.");
                    onError?.Invoke(errorArgs);
                    ReferencePool.Recyle(errorArgs);
                    return;
                }

                AssetItemInfo assetInfo = assetItemDic[crc];
                ResourceLoadTask task = ResourceLoadTask.Create(this, resourceManager.resourceHelper, assetInfo, assetType, priority, groupID);
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

            public void ReloadAssetMap()
            {
                assetItemDic.Clear();
                foreach (var module in resourceManager.localManifest.moduleDic.Values)
                {
                    foreach (var asset in module.assetsDic.Values)
                    {
                        assetItemDic.Add(asset.crc, asset);
                    }
                }
            }

            private void LoadBundle(AssetBundleInfo bundleInfo)
            {
                
            }
        }
    }
}
