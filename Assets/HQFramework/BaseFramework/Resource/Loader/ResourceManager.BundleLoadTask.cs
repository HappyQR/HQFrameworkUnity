using System;

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
                    
                }

                public override TaskStartStatus Start()
                {
                    bool ready = true;

                    for (int i = 0; i < bundleInfo.dependencies.Length; i++)
                    {
                        if (resourceLoader.loadedBundleDic.ContainsKey(bundleInfo.dependencies[i]))
                        {
                            if (resourceLoader.loadedBundleDic[bundleInfo.dependencies[i]] != null)
                            {
                                ready = ready && true;
                            }
                            else
                            {
                                ready = false;
                            }
                        }
                        else
                        {
                            AssetBundleInfo dependencyBundle = null;
                            if (resourceLoader.resourceManager.localManifest.moduleDic[bundleInfo.moduleID].bundleDic.TryGetValue(bundleInfo.dependencies[i], out dependencyBundle))
                            {
                                resourceLoader.LoadBundle(dependencyBundle, priority, groupID);
                            }
                            else // find in other dependence modules
                            {
                                AssetModuleInfo currentModule = resourceLoader.resourceManager.localManifest.moduleDic[bundleInfo.moduleID];
                                for (int j = 0; j < currentModule.dependencies.Length; j++)
                                {
                                    if (resourceLoader.resourceManager.localManifest.moduleDic[currentModule.dependencies[j]].bundleDic.TryGetValue(bundleInfo.dependencies[i], out dependencyBundle))
                                    {
                                        resourceLoader.LoadBundle(dependencyBundle, priority, groupID);
                                        break;
                                    }
                                }
                            }
                            ready = false;
                        }
                    }

                    if (ready)
                    {
                        string bundlePath = resourceLoader.resourceManager.GetBundleFilePath(bundleInfo);
                        resourceHelper.LoadAssetBundle(bundlePath, OnLoadBundleComplete);
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
                    resourceLoader = null;
                    resourceHelper = null;
                    bundleInfo = null;
                    onComplete = null;
                }
            }
        }
    }
}
