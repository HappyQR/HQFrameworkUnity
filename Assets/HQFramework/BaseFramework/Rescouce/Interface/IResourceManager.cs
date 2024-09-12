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

        void SetHelper(IResourceHelper resourceHelper);

        void LoadAsset(uint crc, Type assetType, Action<object> callback);

        void LoadAsset<T>(uint crc, Action<T> callback) where T : class;

        void ReleaseAsset(object asset);
    }
}
