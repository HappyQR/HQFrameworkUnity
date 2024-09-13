namespace HQFramework.Resource
{
    public class HotfixCheckErrorEventArgs
    {
        public readonly string errorMessage;

        public HotfixCheckErrorEventArgs(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }
    }
}
