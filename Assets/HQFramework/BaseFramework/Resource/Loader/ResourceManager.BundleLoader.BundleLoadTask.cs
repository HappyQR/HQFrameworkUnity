using System;
using System.Collections.Generic;

namespace HQFramework.Resource
{
    internal partial class ResourceManager
    {
        private partial class BundleLoader
        {
            private class BundleLoadTask : ResumableTask
            {
                private static int serialID = 0;

                private ResourceManager resourceManager;
                private string bundleName;
                private Queue<string> dependencyQueue = new Queue<string>();

                private Action<BundleLoadCompleteEventArgs> onComplete;
                private Action<BundleLoadErrorEventArgs> onError;

                public event Action<BundleLoadCompleteEventArgs> OnComplete
                {
                    add
                    {
                        onComplete += value;
                    }
                    remove
                    {
                        onComplete -= value;
                    }
                }

                public event Action<BundleLoadErrorEventArgs> OnError
                {
                    add
                    {
                        onError += value;
                    }
                    remove
                    {
                        onError -= value;
                    }
                }

                public static BundleLoadTask Create(ResourceManager resourceManager, string bundleName, int priority, int groupID)
                {
                    BundleLoadTask task = ReferencePool.Spawn<BundleLoadTask>();
                    task.id = serialID++;
                    task.priority = priority;
                    task.groupID = groupID;
                    task.resourceManager = resourceManager;
                    task.bundleName = bundleName;
                    for (int i = 0; i < resourceManager.bundleTable[bundleName].dependencies.Length; i++)
                    {
                        task.dependencyQueue.Enqueue(resourceManager.bundleTable[bundleName].dependencies[i]);
                    }
                    return task;
                }

                public override void OnUpdate()
                {
                    
                }

                public override TaskStartStatus Start()
                {
                    // int loopCount = dependencyQueue.Count;
                    // for (int i = 0; i < loopCount; i++)
                    // {
                    //     string bundleDependency = dependencyQueue.Dequeue();
                    //     if (resourceManager.loadedBundleMap.ContainsKey(bundleDependency))
                    //     {
                    //         if (resourceManager.loadedBundleMap[bundleDependency].Ready)
                    //         {
                    //             resourceManager.loadedBundleMap[bundleDependency].refCount++;
                    //         }
                    //         else
                    //         {
                    //             dependencyQueue.Enqueue(bundleDependency);
                    //         }
                    //     }
                    //     else
                    //     {
                    //         HQAssetBundleConfig dependencyBundle = null;
                    //         if (resourceManager.localManifest.moduleDic[bundleInfo.moduleID].bundleDic.TryGetValue(bundleDependency, out dependencyBundle))
                    //         {
                    //             resourceManager.bundleLoader.LoadBundle(dependencyBundle, priority, groupID);
                    //         }
                    //         else // find in other dependence modules
                    //         {
                    //             HQAssetModuleConfig currentModule = resourceManager.localManifest.moduleDic[bundleInfo.moduleID];
                    //             for (int j = 0; j < currentModule.dependencies.Length; j++)
                    //             {
                    //                 int moduleDependency = currentModule.dependencies[j];
                    //                 if (resourceManager.localManifest.moduleDic[moduleDependency].bundleDic.TryGetValue(bundleDependency, out dependencyBundle))
                    //                 {
                    //                     resourceManager.bundleLoader.LoadBundle(dependencyBundle, priority, groupID);
                    //                     break;
                    //                 }
                    //             }
                    //         }
                    //         dependencyQueue.Enqueue(bundleDependency);
                    //     }
                    // }
                    // if (dependencyQueue.Count == 0)
                    // {
                    //     string bundlePath = resourceManager.GetBundleFilePath(bundleInfo);
                    //     resourceManager.resourceHelper.LoadAssetBundle(bundlePath, OnLoadBundleComplete);
                    //     return TaskStartStatus.InProgress;
                    // }
                    // else
                    // {
                    //     return TaskStartStatus.HasToWait;
                    // }
                    return TaskStartStatus.Done;
                }

                private void OnLoadBundleComplete(object bundleObject)
                {
                    BundleLoadCompleteEventArgs args = BundleLoadCompleteEventArgs.Create(bundleName, bundleObject);
                    onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);

                    status = TaskStatus.Done;
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceManager = null;
                    bundleName = null;
                    onComplete = null;
                    onError = null;
                    dependencyQueue.Clear();
                }
            }
        }
    }
}
