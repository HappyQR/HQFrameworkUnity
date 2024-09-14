namespace HQFramework.Resource
{
    public class HotfixCheckErrorEventArgs
    {
        public readonly int hotfixID;
        public readonly string errorMessage;

        public HotfixCheckErrorEventArgs(int hotfixID, string errorMessage)
        {
            this.hotfixID = hotfixID;
            this.errorMessage = errorMessage;
        }
    }
}
