using System;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class ResourceLoader
        {
            private class ResourceLoadTaskDispatcher : ResumableTaskDispatcher<ResourceLoadTask>
            {
                public ResourceLoadTaskDispatcher(ushort maxConcurrentCount) : base(maxConcurrentCount)
                {
                }

                public void AddResourceLoadCompleteEvent(int taskID, Action<ResourceLoadCompleteEventArgs> completeEvent)
                {
                    if (taskDic.ContainsKey(taskID))
                    {
                        taskDic[taskID].LoadCompleteEvent += completeEvent;
                    }
                }

                public void AddResourceLoadErrorEvent(int taskID, Action<ResourceLoadErrorEventArgs> errorEvent)
                {
                    if (taskDic.ContainsKey(taskID))
                    {
                        taskDic[taskID].LoadErrorEvent += errorEvent;
                    }
                }
            }
        }
    }
}
