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

                public static ResourceLoadTask Create(ResourceLoader resourceLoader, IResourceHelper resourceHelper, AssetItemInfo assetItem, int priority, int groupID)
                {
                    ResourceLoadTask task = ReferencePool.Spawn<ResourceLoadTask>();
                    task.id = serialID++;
                    task.priority = priority;
                    task.groupID = groupID;
                    task.assetItem = assetItem;
                    task.resourceLoader = resourceLoader;
                    task.resourceHelper = resourceHelper;
                    return task;
                }

                public override TaskStartStatus Start()
                {
                    throw new NotImplementedException();
                }

                public override void OnUpdate()
                {
                    throw new NotImplementedException();
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceLoader = null;
                    resourceHelper = null;
                    assetItem = null;
                }
            }
        }
    }
}
