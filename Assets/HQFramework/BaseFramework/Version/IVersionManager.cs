using System;

namespace HQFramework.Version
{
    public interface IVersionManager
    {
        event Action<VersionCheckResult> onVersionCheckCallback;

        event Action<string> onVersionCheckError;

        void SetVersionHelper(IVersionHelper helper);

        void CheckVersion();
    }
}
