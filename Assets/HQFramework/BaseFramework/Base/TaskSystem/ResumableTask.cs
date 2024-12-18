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

        public virtual bool Pause()
        {
            if (status == TaskStatus.InProgress)
            {
                status = TaskStatus.Paused;
                TaskInfo info = new TaskInfo(id, groupID, priority, status);
                onPause?.Invoke(info);
                return true;
            }
            return false;
        }

        public virtual bool Resume()
        {
            if (status == TaskStatus.Paused)
            {
                status = TaskStatus.InProgress;
                TaskInfo info = new TaskInfo(id, groupID, priority, status);
                onResume?.Invoke(info);
                return true;
            }
            return false;
        }

        protected override void OnRecyle()
        {
            base.OnRecyle();
            onPause = null;
            onResume = null;
        }
    }
}
