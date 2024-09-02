namespace HQFramework
{
    public struct TaskInfo
    {
        public readonly int id;
        public readonly int groupID;
        public readonly int priority;
        public readonly TaskStatus status;

        public TaskInfo(int id, int groupID, int priority, TaskStatus status)
        {
            this.id = id;
            this.groupID = groupID;
            this.priority = priority;
            this.status = status;
        }
    }
}
