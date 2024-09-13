using System;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class ResourceLoadTask : TaskBase
        {
            private uint crc;
            private Type assetType;
            private object asset;
            private ResourceManager manager;

            public static ResourceLoadTask Create(ResourceManager manager, uint crc, Type assetType)
            {
                ResourceLoadTask task = ReferencePool.Spawn<ResourceLoadTask>();
                task.crc = crc;
                task.assetType = assetType;
                task.manager = manager;
                return task;
            }

            public override TaskStartStatus Start()
            {
                throw new NotImplementedException();
            }

            public override void OnUpdate()
            {
                
            }

            protected override void OnRecyle()
            {
                base.OnRecyle();
                crc = 0;
                assetType = null;
                asset = null;
                manager = null;
            }
        }
    }
}
