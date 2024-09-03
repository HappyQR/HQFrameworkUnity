using System;
using System.Collections;

namespace HQFramework.Coroutine
{
    public interface ICoroutineManager
    {
        int StartCoroutine(IEnumerator func, int groupID, int priority);

        bool StopCoroutine(int id);

        void AddCoroutineStopEvent(int id, Action<TaskInfo> onCancel);

        public void AddCoroutineCompleteEvent(int id, Action<TaskInfo> onCompleted);

        public void AddCoroutinePauseEvent(int id, Action<TaskInfo> onPause);

        public void AddCoroutineResumeEvent(int id, Action<TaskInfo> onResume);

        bool PauseCoroutine(int id);

        bool ResumeCoroutine(int id);

        int StopCoroutines(int groupID);

        int PauseCoroutines(int groupID);

        int ResumeCoroutines(int groupID);

        void StopAllCoroutines();

        int DelayInvoke(float delayTime, System.Action func, bool realtime = true);

        int RepeatInvoke(float interval, System.Action func, int repeatCount = -1, bool realtime = true);
    }
}
