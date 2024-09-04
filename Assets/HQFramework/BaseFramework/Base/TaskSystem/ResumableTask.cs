using System;

namespace HQFramework
{
    public abstract class ResumableTask : TaskBase
    {
        protected Action<TaskInfo> onPause;
        protected Action<TaskInfo> onResume;

        public event Action<TaskInfo> PauseEvent
        {
            add { onPause += value; }
            remove { onPause -= value; }
        }

        public event Action<TaskInfo> ResumeEvent
        {
            add { onResume += value; }
            remove { onResume -= value; }
        }

        public virtual void Pause()
        {
            if (status == TaskStatus.InProgress)
            {
                status = TaskStatus.Paused;
                TaskInfo info = new TaskInfo(ID, GroupID, Priority, Status);
                onPause?.Invoke(info);
            }
        }

        public virtual void Resume()
        {
            if (status == TaskStatus.Paused)
            {
                status = TaskStatus.InProgress;
                TaskInfo info = new TaskInfo(ID, GroupID, Priority, Status);
                onResume?.Invoke(info);
            }
        }

        protected override void OnRecyle()
        {
            base.OnRecyle();
            onPause = null;
            onResume = null;
        }
    }
}
