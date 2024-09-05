namespace HQFramework
{
    public class HotfixCheckEventArgs
    {
        public readonly bool isLatest;
        public readonly bool forceUpdate;
        public readonly string releaseNote;
        public readonly float totalSize;

        public HotfixCheckEventArgs(bool isLatest, bool forceUpdate, string releaseNote, float totalSize)
        {
            this.isLatest = isLatest;
            this.forceUpdate = forceUpdate;
            this.releaseNote = releaseNote;
            this.totalSize = totalSize;
        }
    }
}
