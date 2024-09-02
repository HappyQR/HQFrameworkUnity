namespace HQFramework
{
    public enum TaskStatus : byte
    {
        Waiting,
        InProgress,
        Paused,
        Error,
        Canceled,
        Done
    }
}
