using System.Collections;

namespace HQFramework.Coroutine
{
    internal partial class CoroutineManager
    {
        private sealed class CoroutineTask : ResumableTask
        {
            private static int serialID;

            private IEnumerator func;
            private object current;
            private bool isDone;

            public static CoroutineTask Create(IEnumerator func, int groupID, int priority)
            {
                CoroutineTask task = ReferencePool.Spawn<CoroutineTask>();
                task.id = serialID++;
                task.priority = priority;
                task.groupID = groupID;
                task.func = func;
                return task;
            }

            public override TaskStartStatus Start()
            {
                status = TaskStatus.InProgress;
                return TaskStartStatus.InProgress;
            }

            public override void OnUpdate()
            {
                if (current is IAsyncOperation && !(current as IAsyncOperation).isDone)
                {
                    return;
                }

                isDone = !func.MoveNext();
                if (isDone)
                {
                    status = TaskStatus.Done;
                    TaskInfo info = new TaskInfo(ID, GroupID, Priority, Status);
                    onCompleted?.Invoke(info);
                }
                else
                {
                    current = func.Current;
                }
            }

            protected override void OnRecyle()
            {
                base.OnRecyle();
                func = null;
                current = null;
                isDone = false;
            }
        }
    }
}
