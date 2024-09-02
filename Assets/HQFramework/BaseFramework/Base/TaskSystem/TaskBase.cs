using System;

namespace HQFramework
{
    public abstract class TaskBase : IReference
    {
        protected int id;
        protected int groupID;
        protected int priority;
        protected TaskStatus status;

        protected Action<TaskInfo> onCancel;
        protected Action<TaskInfo> onCompleted;

        public event Action<TaskInfo> CancelEvent
        {
            add { onCancel += value; }
            remove { onCancel -= value; }
        }

        public event Action<TaskInfo> CompleteEvent
        {
            add { onCompleted += value; }
            remove { onCompleted -= value; }
        }

        public int ID => id;
        public int GroupID => groupID;
        public int Priority => priority;
        public TaskStatus Status => status;

        public abstract TaskStartStatus Start();
        public abstract void OnUpdate();

        public virtual void Cancel()
        {
            status = TaskStatus.Canceled;
            TaskInfo info = new TaskInfo(ID, GroupID, Priority, Status);
            onCancel?.Invoke(info);
        }

        public virtual void OnRecyle()
        {
            status = TaskStatus.Waiting;
            onCancel = null;
            onCompleted = null;
        }
    }
}
