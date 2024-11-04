using System;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class BundleLoader
        {
            private class BundleLoadTaskDispatcher : ResumableTaskDispatcher<BundleLoadTask>
            {
                public BundleLoadTaskDispatcher(ushort maxConcurrentCount) : base(maxConcurrentCount)
                {
                }

                public void AddBundleLoadCompleteCallback(int taskID, Action<BundleLoadCompleteEventArgs> onComplete)
                {
                    if (taskDic.ContainsKey(taskID))
                    {
                        taskDic[taskID].OnComplete += onComplete;
                    }
                }
            }
        }
    }
}
