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
                private AssetBundleInfo bundleInfo;
                private Queue<string> dependencyQueue = new Queue<string>();

                private Action<BundleLoadCompleteEventArgs> onComplete;

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

                public static BundleLoadTask Create(ResourceManager resourceManager, AssetBundleInfo bundleInfo, int priority, int groupID)
                {
                    BundleLoadTask task = ReferencePool.Spawn<BundleLoadTask>();
                    task.id = serialID++;
                    task.priority = priority;
                    task.groupID = groupID;
                    task.resourceManager = resourceManager;
                    task.bundleInfo = bundleInfo;
                    for (int i = 0; i < task.bundleInfo.dependencies.Length; i++)
                    {
                        task.dependencyQueue.Enqueue(task.bundleInfo.dependencies[i]);
                    }
                    return task;
                }

                public override void OnUpdate()
                {
                    
                }

                public override TaskStartStatus Start()
                {
                    int loopCount = dependencyQueue.Count;
                    for (int i = 0; i < loopCount; i++)
                    {
                        string bundleDependency = dependencyQueue.Dequeue();
                        if (resourceManager.loadedBundleMap.ContainsKey(bundleDependency))
                        {
                            if (resourceManager.loadedBundleMap[bundleDependency].Ready)
                            {
                                resourceManager.loadedBundleMap[bundleDependency].refCount++;
                            }
                            else
                            {
                                dependencyQueue.Enqueue(bundleDependency);
                            }
                        }
                        else
                        {
                            AssetBundleInfo dependencyBundle = null;
                            if (resourceManager.localManifest.moduleDic[bundleInfo.moduleID].bundleDic.TryGetValue(bundleDependency, out dependencyBundle))
                            {
                                resourceManager.bundleLoader.LoadBundle(dependencyBundle, priority, groupID);
                            }
                            else // find in other dependence modules
                            {
                                AssetModuleInfo currentModule = resourceManager.localManifest.moduleDic[bundleInfo.moduleID];
                                for (int j = 0; j < currentModule.dependencies.Length; j++)
                                {
                                    int moduleDependency = currentModule.dependencies[j];
                                    if (resourceManager.localManifest.moduleDic[moduleDependency].bundleDic.TryGetValue(bundleDependency, out dependencyBundle))
                                    {
                                        resourceManager.bundleLoader.LoadBundle(dependencyBundle, priority, groupID);
                                        break;
                                    }
                                }
                            }
                            dependencyQueue.Enqueue(bundleDependency);
                        }
                    }
                    if (dependencyQueue.Count == 0)
                    {
                        string bundlePath = resourceManager.GetBundleFilePath(bundleInfo);
                        resourceManager.resourceHelper.LoadAssetBundle(bundlePath, OnLoadBundleComplete);
                        return TaskStartStatus.InProgress;
                    }
                    else
                    {
                        return TaskStartStatus.HasToWait;
                    }
                }

                private void OnLoadBundleComplete(object bundleObject)
                {
                    BundleLoadCompleteEventArgs args = BundleLoadCompleteEventArgs.Create(bundleInfo.bundleName, bundleObject);
                    onComplete?.Invoke(args);
                    ReferencePool.Recyle(args);

                    status = TaskStatus.Done;
                }

                protected override void OnRecyle()
                {
                    base.OnRecyle();
                    resourceManager = null;
                    bundleInfo = null;
                    onComplete = null;
                    dependencyQueue.Clear();
                }
            }
        }
    }
}
