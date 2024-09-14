using System.Collections.Generic;

namespace HQFramework.Resource
{
    public class HotfixCheckCompleteEventArgs
    {
        public readonly int hotfixID;
        public readonly bool isLatest;
        public readonly bool forceUpdate;
        public readonly string releaseNote;
        public readonly float totalSize;

        public HotfixCheckCompleteEventArgs(int hotfixID, bool isLatest, bool forceUpdate, string releaseNote, float totalSize)
        {
            this.hotfixID = hotfixID;
            this.isLatest = isLatest;
            this.forceUpdate = forceUpdate;
            this.releaseNote = releaseNote;
            this.totalSize = totalSize;
        }
    }
}
