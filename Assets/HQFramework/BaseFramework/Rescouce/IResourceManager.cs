using System;

namespace HQFramework.Resource
{
    public interface IResourceManager
    {
        AssetHotfixMode HotfixMode { get; }

        string PersistentDir { get; }

        string BuiltinDir { get; }

        string HotfixUrl { get; }

        string HotfixManifestUrl { get; }

        event Action<HotfixCheckCompleteEventArgs> HotfixCheckCompleteEvent;

        event Action<HotfixCheckErrorEventArgs> HotfixCheckErrorEvent;

        event Action<HotfixDownloadUpdateEventArgs> HotfixDownloadUpdateEvent;

        event Action<HotfixDownloadErrorEventArgs> HotfixDownloadErrorEvent;

        event Action<HotfixDownloadCompleteEventArgs> HotfixDownloadCompleteEvent;

        void SetHelper(IResourceHelper resourceHelper);

        void CheckHotfix();

        void StartHotfix();

        HotfixCheckCompleteEventArgs CheckModuleHotfix(int moduleID);

        int StartModuleHotfix(int moduleID);

        void LoadAsset(uint crc, Type assetType, Action<object> callback);

        void LoadAsset<T>(uint crc, Action<T> callback) where T : class;

        void ReleaseAsset(object asset);
    }
}
