using System;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private class ResourceLoadTaskInfo
        {
            public uint crc;
            public Type assetType;
            public Action<ResourceLoadCompleteEventArgs> onComplete;
            public Action<ResourceLoadErrorEventArgs> onError;
            public int priority;
            public int groupID;

            public ResourceLoadTaskInfo(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
            {
                this.crc = crc;
                this.assetType = assetType;
                this.onComplete = onComplete;
                this.onError = onError;
                this.priority = priority;
                this.groupID = groupID;
            }
        }
    }
}
