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

                public void AddResourceLoadCompleteEvent(int taskID, Action<ResourceLoadCompleteEventArgs> onComplete)
                {
                    if (taskDic.ContainsKey(taskID))
                    {
                        taskDic[taskID].OnComplete += onComplete;
                    }
                }

                public void AddResourceLoadErrorEvent(int taskID, Action<ResourceLoadErrorEventArgs> onError)
                {
                    if (taskDic.ContainsKey(taskID))
                    {
                        taskDic[taskID].OnError += onError;
                    }
                }
            }
        }
    }
}
