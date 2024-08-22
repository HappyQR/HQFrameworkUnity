namespace HQFramework.Version
{
    public class VersionCheckResult
    {
        public enum UpdateType : byte
        {
            Latest,
            SuggestedUpdate,
            ForceUpdate
        }

        public readonly UpdateType updateType;
        public readonly VersionInfo newVersionInfo;

        public VersionCheckResult(UpdateType updateType, VersionInfo newVersionInfo)
        {
            this.updateType = updateType;
            this.newVersionInfo = newVersionInfo;
        }
    }
}
