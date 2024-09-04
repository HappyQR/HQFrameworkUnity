using System;
using System.Collections;

namespace HQFramework.Coroutine
{
    internal sealed partial class CoroutineManager : HQModuleBase, ICoroutineManager
    {
        private CoroutineDispatcher dispatcher;
        private static readonly ushort maxConcurrentCount = 128;

        public override byte Priority => byte.MaxValue;

        protected override void OnInitialize()
        {
            dispatcher = new CoroutineDispatcher(maxConcurrentCount);
        }

        protected override void OnUpdate()
        {
            dispatcher.ProcessTasks();
        }

        protected override void OnShutdown()
        {
            dispatcher.CancelAllTasks();
        }

        public int StartCoroutine(IEnumerator func, int groupID = 0, int priority = 0)
        {
            CoroutineTask task = CoroutineTask.Create(func, groupID, priority);
            return dispatcher.AddTask(task);
        }

        public void AddCoroutineStopEvent(int id, Action<TaskInfo> onCancel)
        {
            dispatcher.AddTaskCancelEvent(id, onCancel);
        }

        public void AddCoroutineCompleteEvent(int id, Action<TaskInfo> onCompleted)
        {
            dispatcher.AddTaskCompleteEvent(id, onCompleted);
        }

        public void AddCoroutinePauseEvent(int id, Action<TaskInfo> onPause)
        {
            dispatcher.AddTaskPauseEvent(id, onPause);
        }

        public void AddCoroutineResumeEvent(int id, Action<TaskInfo> onResume)
        {
            dispatcher.AddTaskResumeEvent(id, onResume);
        }

        public bool PauseCoroutine(int id)
        {
            return dispatcher.PauseTask(id);
        }

        public int PauseCoroutines(int groupID)
        {
            return dispatcher.PauseTasks(groupID);
        }

        public bool ResumeCoroutine(int id)
        {
            return dispatcher.ResumeTask(id);
        }

        public int ResumeCoroutines(int groupID)
        {
            return dispatcher.CancelTasks(groupID);
        }

        public bool StopCoroutine(int id)
        {
            return dispatcher.CancelTask(id);
        }

        public int StopCoroutines(int groupID)
        {
            return dispatcher.CancelTasks(groupID);
        }

        public void StopAllCoroutines()
        {
            dispatcher.CancelAllTasks();
        }

        public int DelayInvoke(float delayTime, Action func, bool realtime = true)
        {
            return StartCoroutine(DelayInvokeInternal(delayTime, func, realtime));
        }

        public int RepeatInvoke(float interval, Action func, int repeatCount = -1, bool realtime = true)
        {
            return StartCoroutine(RepeatInvokeInternal(interval, func, repeatCount, realtime));
        }

        private IEnumerator DelayInvokeInternal(float delayTime, System.Action func, bool realtime)
        {
            IAsyncOperation wait = realtime ? new YieldSecondsRealtime(delayTime) : new YieldSecondsLogic(delayTime);
            yield return wait;

            func?.Invoke();
        }

        private IEnumerator RepeatInvokeInternal(float interval, System.Action func, int repeatCount, bool realtime)
        {
            IAsyncOperation wait = realtime ? new YieldSecondsRealtime(interval) : new YieldSecondsLogic(interval);
            if (repeatCount > 0)
            {
                for (int i = 0; i < repeatCount; i++)
                {
                    func?.Invoke();
                    yield return wait;
                }
            }
            else
            {
                while (true)
                {
                    func?.Invoke();
                    yield return wait;
                }
            }
        }
    }
}
