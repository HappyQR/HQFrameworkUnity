namespace HQFramework.Hotfix
{
    public class HotfixErrorEventArgs
    {
        public string ErrorMessage { get; private set; }

        public HotfixErrorEventArgs(string message)
        {
            ErrorMessage = message;
        }
    }
}
