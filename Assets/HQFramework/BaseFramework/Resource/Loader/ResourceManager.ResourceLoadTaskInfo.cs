using System;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class ResourceLoadTaskInfo : IReference
        {
            public uint crc;
            public Type assetType;
            public Action<ResourceLoadCompleteEventArgs> onComplete;
            public Action<ResourceLoadErrorEventArgs> onError;
            public int priority;
            public int groupID;

            public static ResourceLoadTaskInfo Create(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
            {
                ResourceLoadTaskInfo info = ReferencePool.Spawn<ResourceLoadTaskInfo>(); 
                info.crc = crc;
                info.assetType = assetType;
                info.onComplete = onComplete;
                info.onError = onError;
                info.priority = priority;
                info.groupID = groupID;
                return info;
            }

            void IReference.OnRecyle()
            {
                crc = 0;
                assetType = null;
                onComplete = null;
                onError = null;
                priority = 0;
                groupID = 0;
            }
        }
    }
}
