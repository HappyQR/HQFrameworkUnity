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

        bool RemoveTask(int id);

        int RemoveTasks(int groupID);

        void RemoveAllTasks();

        void ProcessTasks();
    }
}
