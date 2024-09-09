using System.Collections.Generic;

namespace HQFramework.Hotfix
{
    public class HotfixCheckEventArgs
    {
        public readonly bool isLatest;
        public readonly bool forceUpdate;
        public readonly string releaseNote;
        public readonly float totalSize;
        internal readonly List<HotfixManager.HotfixPatch> patchList;

        internal HotfixCheckEventArgs(bool isLatest, bool forceUpdate, string releaseNote, float totalSize, List<HotfixManager.HotfixPatch> patchList)
        {
            this.isLatest = isLatest;
            this.forceUpdate = forceUpdate;
            this.releaseNote = releaseNote;
            this.totalSize = totalSize;
            this.patchList = patchList;
        }
    }
}
