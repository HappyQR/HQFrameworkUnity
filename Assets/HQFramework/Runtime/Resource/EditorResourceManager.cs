using System;
using System.Collections;
using System.Collections.Generic;
using HQFramework.Coroutine;
using HQFramework.Download;
using HQFramework.Resource;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace HQFramework.Runtime
{
    public class EditorResourceManager : IResourceManager
    {
        private Dictionary<uint, string> assetPathMap;

        public EditorResourceManager(string assetRootDir)
        {
            string[] assetGuids = AssetDatabase.FindAssets("", new string[] { assetRootDir });
            assetPathMap = new Dictionary<uint, string>(assetGuids.Length);
            for (int i = 0; i < assetGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                assetPathMap.Add(Utility.CRC32.ComputeCrc32(assetGuids[i]), path);
            }
        }

        public string PersistentDir => throw new NotSupportedException("Not Supported Under Editor Resource Manager.");

        public string BuiltinDir => throw new NotSupportedException("Not Supported Under Editor Resource Manager.");

        public bool HasModule(int moduleID)
        {
            return true;
        }

        public void InstantiateAsset(uint crc, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            if (!assetPathMap.ContainsKey(crc))
            {
                ResourceLoadErrorEventArgs args = ResourceLoadErrorEventArgs.Create(crc, null, null);
                onError?.Invoke(args);
                ReferencePool.Recyle(args);
                return;
            }

            string assetPath = assetPathMap[crc];
            UnityObject asset = AssetDatabase.LoadAllAssetsAtPath(assetPath)[0];
            asset = UnityObject.Instantiate(asset);
            ResourceLoadCompleteEventArgs completeEventArgs = ResourceLoadCompleteEventArgs.Create(crc, asset);
            onComplete?.Invoke(completeEventArgs);
            ReferencePool.Recyle(completeEventArgs);
        }

        public void InstantiateAsset(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            InstantiateAsset(crc, onComplete, onError, priority, groupID);
        }

        public void InstantiateAsset<T>(uint crc, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class
        {
            InstantiateAsset(crc, onComplete, onError, priority, groupID);
        }

        public void InstantiateAsset(string path, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            InstantiateAsset(Utility.CRC32.ComputeCrc32(path), onComplete, onError, priority, groupID);
        }

        public void InstantiateAsset(string path, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            InstantiateAsset(Utility.CRC32.ComputeCrc32(path), onComplete, onError, priority, groupID);
        }

        public void InstantiateAsset<T>(string path, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class
        {
            InstantiateAsset(Utility.CRC32.ComputeCrc32(path), onComplete, onError, priority, groupID);
        }

        public void LoadAsset(uint crc, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            InstantiateAsset(crc, onComplete, onError, priority, groupID);
        }

        public void LoadAsset(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            InstantiateAsset(crc, assetType, onComplete, onError, priority, groupID);
        }

        public void LoadAsset<T>(uint crc, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class
        {
            InstantiateAsset<T>(crc, onComplete, onError, priority, groupID);
        }

        public void LoadAsset(string path, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            InstantiateAsset(path, onComplete, onError, priority, groupID);
        }

        public void LoadAsset(string path, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID)
        {
            InstantiateAsset(path, assetType, onComplete, onError, priority, groupID);
        }

        public void LoadAsset<T>(string path, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class
        {
            InstantiateAsset<T>(path, onComplete, onError, priority, groupID);
        }

        public void ReleaseAsset(object asset)
        {
            if (asset is GameObject)
            {
                UnityObject.Destroy(asset as GameObject);
            }
        }

        public void DecompressBuiltinAssets(Action onComplete)
        {
            HQFrameworkEngine.GetModule<ICoroutineManager>().StartCoroutine(DecompressBuiltinAssetsInternal(onComplete));
        }

        private IEnumerator DecompressBuiltinAssetsInternal(Action onComplete)
        {
            yield return new WaitForSecondsRealtime(1);
            onComplete?.Invoke();
        }

        public void AddHotfixCheckCompleteEvent(int hotfixID, Action<HotfixCheckCompleteEventArgs> onHotfixCheckComplete)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void AddHotfixCheckErrorEvent(int hotfixID, Action<HotfixCheckErrorEventArgs> onHotfixCheckError)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void AddHotfixDownloadCancelEvent(int hotfixID, Action<HotfixDownloadCancelEventArgs> onHotfixCancel)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void AddHotfixDownloadCompleteEvent(int hotfixID, Action<HotfixDownloadCompleteEventArgs> onHotfixComplete)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void AddHotfixDownloadErrorEvent(int hotfixID, Action<HotfixDownloadErrorEventArgs> onHotfixError)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void AddHotfixDownloadPauseEvent(int hotfixID, Action<HotfixDownloadPauseEventArgs> onHotfixPause)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void AddHotfixDownloadResumeEvent(int hotfixID, Action<HotfixDownloadResumeEventArgs> onHotfixResume)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void AddHotfixDownloadUpdateEvent(int hotfixID, Action<HotfixDownloadUpdateEventArgs> onHotfixUpdate)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void CancelHotfix(int hotfixID)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public AssetItemInfo[] GetLoadedAssetInfo()
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public AssetBundleInfo[] GetLoadedBundleInfo()
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public int LaunchHotfix()
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public int LaunchHotfixCheck()
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public int ModuleHotfix(int moduleID)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public int ModuleHotfixCheck(int moduleID)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void PauseHotfix(int hotfixID)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void ResumeHotfix(int hotfixID)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void SetDownloadManager(IDownloadManager downloadManager)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }

        public void SetHelper(IResourceHelper resourceHelper)
        {
            throw new NotSupportedException("Not Supported Under Editor Resource Manager.");
        }
    }
}
