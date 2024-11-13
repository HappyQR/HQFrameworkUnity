using System;
using System.Collections;
using HQFramework.Coroutine;

namespace HQFramework.Runtime
{
    public class CoroutineComponent : BaseComponent
    {
        private ICoroutineManager coroutineManager;

        private void Start()
        {
            coroutineManager = HQFrameworkEngine.GetModule<ICoroutineManager>();
        }

        public void AddHQCoroutineCompleteEvent(int id, Action<TaskInfo> onCompleted)
        {
            coroutineManager.AddCoroutineCompleteEvent(id, onCompleted);
        }

        public void AddHQCoroutinePauseEvent(int id, Action<TaskInfo> onPause)
        {
            coroutineManager.AddCoroutinePauseEvent(id, onPause);
        }

        public void AddHQCoroutineResumeEvent(int id, Action<TaskInfo> onResume)
        {
            coroutineManager.AddCoroutineResumeEvent(id, onResume);
        }

        public void AddHQCoroutineStopEvent(int id, Action<TaskInfo> onCancel)
        {
            coroutineManager.AddCoroutineStopEvent(id, onCancel);
        }

        public int DelayInvoke(float delayTime, Action func, bool realtime = true)
        {
            return coroutineManager.DelayInvoke(delayTime, func, realtime);
        }

        public bool PauseHQCoroutine(int id)
        {
            return coroutineManager.PauseCoroutine(id);
        }

        public int PauseHQCoroutines(int groupID)
        {
            return coroutineManager.PauseCoroutines(groupID);
        }

        public int RepeatInvoke(float interval, Action func, int repeatCount = -1, bool realtime = true)
        {
            return coroutineManager.RepeatInvoke(interval, func, repeatCount, realtime);
        }

        public bool ResumeHQCoroutine(int id)
        {
            return coroutineManager.ResumeCoroutine(id);
        }

        public int ResumeHQCoroutines(int groupID)
        {
            return coroutineManager.ResumeCoroutines(groupID);
        }

        public int StartHQCoroutine(IEnumerator func, int groupID = 0, int priority = 0)
        {
            return coroutineManager.StartCoroutine(func, groupID, priority);
        }

        public bool StopHQCoroutine(int id)
        {
            return coroutineManager.StopCoroutine(id);
        }

        public int StopHQCoroutines(int groupID)
        {
            return coroutineManager.StopCoroutines(groupID);
        }

        public void StopAllHQCoroutines()
        {
            coroutineManager.StopAllCoroutines();
        }
    }
}
