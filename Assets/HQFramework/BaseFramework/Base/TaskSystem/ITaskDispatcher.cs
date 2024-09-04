namespace HQFramework
{
    public interface ITaskDispatcher<T> where T : TaskBase
    {
        ushort MaxConcurrentCount
        {
            get;
        }

        int WorkingCount
        {
            get;
        }

        int WaitingCount
        {
            get;
        }

        int AddTask(T task);

        bool CancelTask(int id);

        int CancelTasks(int groupID);

        void CancelAllTasks();

        void ProcessTasks();
    }
}
