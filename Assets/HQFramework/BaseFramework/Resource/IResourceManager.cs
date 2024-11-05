using System;

namespace HQFramework.Resource
{
    public interface IResourceManager
    {
        string PersistentDir { get; }

        string BuiltinDir { get; }

        void SetHelper(IResourceHelper resourceHelper);

        int LaunchHotfixCheck();

        int LaunchHotfix();

        int ModuleHotfixCheck(int moduleID);

        int ModuleHotfix(int moduleID);

        void PauseHotfix(int hotfixID);

        void ResumeHotfix(int hotfixID);

        void CancelHotfix(int hotfixID);

        void AddHotfixCheckErrorEvent(int hotfixID, Action<HotfixCheckErrorEventArgs> onHotfixCheckError);

        void AddHotfixCheckCompleteEvent(int hotfixID, Action<HotfixCheckCompleteEventArgs> onHotfixCheckComplete);

        void AddHotfixDownloadUpdateEvent(int hotfixID, Action<HotfixDownloadUpdateEventArgs> onHotfixUpdate);

        void AddHotfixDownloadErrorEvent(int hotfixID, Action<HotfixDownloadErrorEventArgs> onHotfixError);

        void AddHotfixDownloadPauseEvent(int hotfixID, Action<HotfixDownloadPauseEventArgs> onHotfixPause);

        void AddHotfixDownloadResumeEvent(int hotfixID, Action<HotfixDownloadResumeEventArgs> onHotfixResume);

        void AddHotfixDownloadCancelEvent(int hotfixID, Action<HotfixDownloadCancelEventArgs> onHotfixCancel);

        void AddHotfixDownloadCompleteEvent(int hotfixID, Action<HotfixDownloadCompleteEventArgs> onHotfixComplete);

        bool HasModule(int moduleID);

        void LoadAsset(uint crc, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID);

        void LoadAsset<T>(uint crc, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class;

        void LoadAsset(string path, Type assetType, Action<ResourceLoadCompleteEventArgs> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID);

        void LoadAsset<T>(string path, Action<ResourceLoadCompleteEventArgs<T>> onComplete, Action<ResourceLoadErrorEventArgs> onError, int priority, int groupID) where T : class;

        void ReleaseAsset(object asset);

        BundleData[] GetLoadedBundleData();

        AssetData[] GetLoadedAssetData();
    }
}
