using System;

namespace HQFramework.Resource
{
    public interface IResourceManager
    {
        AssetHotfixMode HotfixMode { get; }

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

        void LoadAsset(uint crc, Type assetType, Action<object> callback);

        void LoadAsset<T>(uint crc, Action<T> callback) where T : class;

        void ReleaseAsset(object asset);
    }
}
