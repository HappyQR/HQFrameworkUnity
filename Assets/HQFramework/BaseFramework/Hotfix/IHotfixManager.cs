using System;

namespace HQFramework.Hotfix
{
    public interface IHotfixManager
    {
        event Action<HotfixCheckEventArgs> onHotfixCheckDone;

        event Action<HotfixCheckErrorEventArgs> onHotfixCheckError;

        event Action<HotfixErrorEventArgs> onHotfixError;

        event Action<HotfixUpdateEventArgs> onHotfixUpdate;

        event Action onHotfixDone;

        void StartHotfixCheck();

        void StartHotfix();
    }
}
