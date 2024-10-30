namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class ResourceLoader
        {
            private class BundleLoadTask : ResumableTask
            {
                private static int serialID = 0;

                private ResourceLoader resourceLoader;
                private IResourceHelper resourceHelper;
                private AssetBundleInfo bundleInfo;

                public static BundleLoadTask Create(ResourceLoader resourceLoader, IResourceHelper resourceHelper, AssetBundleInfo bundleInfo, int priority, int groupID)
                {
                    BundleLoadTask task = ReferencePool.Spawn<BundleLoadTask>();
                    task.id = serialID++;
                    task.priority = priority;
                    task.groupID = groupID;
                    task.resourceLoader = resourceLoader;
                    task.resourceHelper = resourceHelper;
                    task.bundleInfo = bundleInfo;
                    return task;
                }

                public override void OnUpdate()
                {
                    throw new System.NotImplementedException();
                }

                public override TaskStartStatus Start()
                {
                    throw new System.NotImplementedException();
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceLoader = null;
                    resourceHelper = null;
                    bundleInfo = null;
                }
            }
        }
    }
}
