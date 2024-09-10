namespace HQFramework.Hotfix
{
    public class HotfixUpdateEventArgs : IReference
    {
        public float Progress { get; private set; }

        public static HotfixUpdateEventArgs Create(float progress)
        {
            HotfixUpdateEventArgs args = ReferencePool.Spawn<HotfixUpdateEventArgs>();
            args.Progress = progress;
            return args;
        }

        void IReference.OnRecyle()
        {
            Progress = 0;
        }
    }
}
