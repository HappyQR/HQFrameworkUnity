using System;
using System.Collections.Generic;

namespace HQFramework
{
    public abstract class ResumableTaskDispatcher<T> : ITaskDispatcher<T> where T : ResumableTask
    {
        private Queue<T> workingQueue;
        private LinkedList<T> taskList;
        protected Dictionary<int, T> taskDic;
        private readonly ushort maxConcurrentCount;

        public ushort MaxConcurrentCount => maxConcurrentCount;

        public int WorkingCount => workingQueue.Count;

        public int WaitingCount => taskList.Count;

        public ResumableTaskDispatcher(ushort maxConcurrentCount)
        {
            this.maxConcurrentCount = maxConcurrentCount;
            workingQueue = new Queue<T>(maxConcurrentCount);
            taskList = new LinkedList<T>();
            taskDic = new Dictionary<int, T>();
            ReferencePool.RegisterPool(typeof(T), maxConcurrentCount);
        }
        
        public int AddTask(T task)
        {
            LinkedListNode<T> node = taskList.Last;
            while (node != null)
            {
                if (node.Value.Priority <= task.Priority)
                {
                    break;
                }
                node = node.Previous;
            }

            if (node != null)
            {
                taskList.AddAfter(node, task);
            }
            else
            {
                taskList.AddFirst(task);
            }

            taskDic.Add(task.ID, task);
            return task.ID;
        }

        public bool CancelTask(int id)
        {
            if (taskDic.ContainsKey(id))
            {
                taskDic[id].Cancel();
                return true;
            }

            return false;
        }

        public int CancelTasks(int groupID)
        {
            int resultCount = 0;
            LinkedListNode<T> node = taskList.First;
            while (node != null)
            {
                if (node.Value.GroupID == groupID)
                {
                    node.Value.Cancel();
                    resultCount++;
                }
                node = node.Next;
            }

            int count = workingQueue.Count;
            for (int i = 0; i < count; i++)
            {
                T task = workingQueue.Dequeue();
                if (task.GroupID == groupID)
                {
                    task.Cancel();
                    resultCount++;
                }
                workingQueue.Enqueue(task);
            }

            return resultCount;
        }

        public void CancelAllTasks()
        {
            LinkedListNode<T> node = taskList.First;
            while (node != null)
            {
                node.Value.Cancel();
                node = node.Next;
            }

            int count = workingQueue.Count;
            for (int i = 0; i < count; i++)
            {
                T task = workingQueue.Dequeue();
                task.Cancel();
                workingQueue.Enqueue(task);
            }
        }

        public bool PauseTask(int id)
        {
            int count = workingQueue.Count;
            for (int i = 0; i < count; i++)
            {
                T task = workingQueue.Dequeue();
                if (task.ID == id)
                {
                    bool result = task.Pause();
                    workingQueue.Enqueue(task);
                    return result;
                }
                workingQueue.Enqueue(task);
            }

            return false;
        }

        public int PauseTasks(int groupID)
        {
            int resultCount = 0;
            int count = workingQueue.Count;
            for (int i = 0; i < count; i++)
            {
                T task = workingQueue.Dequeue();
                if (task.GroupID == groupID)
                {
                    if (task.Pause())
                    {
                        resultCount++;
                    }
                }
                workingQueue.Enqueue(task);
            }

            return resultCount;
        }

        public bool ResumeTask(int id)
        {
            int count = workingQueue.Count;
            for (int i = 0; i < count; i++)
            {
                T task = workingQueue.Dequeue();
                if (task.ID == id)
                {
                    bool result = task.Resume();
                    workingQueue.Enqueue(task);
                    return result;
                }
                workingQueue.Enqueue(task);
            }

            return false;
        }

        public int ResumeTasks(int groupID)
        {
            int resultCount = 0;
            int count = workingQueue.Count;
            for (int i = 0; i < count; i++)
            {
                T task = workingQueue.Dequeue();
                if (task.GroupID == groupID)
                {
                    if (task.Resume())
                    {
                        resultCount++;
                    }
                }
                workingQueue.Enqueue(task);
            }

            return resultCount;
        }

        public void ProcessTasks()
        {
            // process waiting tasks
            LinkedListNode<T> current = taskList.First;
            while (workingQueue.Count < maxConcurrentCount && current != null)
            {
                LinkedListNode<T> next = current.Next;
                if (current.Value.Status == TaskStatus.Canceled)
                {
                    ReferencePool.Recyle(current.Value);
                    taskList.Remove(current);
                    taskDic.Remove(current.Value.ID);
                    current = next;
                }
                else
                {
                    TaskStartStatus status = current.Value.Start();
                    switch (status)
                    {
                        case TaskStartStatus.Done:
                        case TaskStartStatus.Error:
                            ReferencePool.Recyle(current.Value);
                            taskList.Remove(current);
                            taskDic.Remove(current.Value.ID);
                            current = next;
                            break;
                        case TaskStartStatus.InProgress:
                            workingQueue.Enqueue(current.Value);
                            taskList.Remove(current);
                            current = next;
                            break;
                        case TaskStartStatus.HasToWait:
                            current = next;
                            break;
                    }
                }
            }

            // process running tasks
            int count = workingQueue.Count;
            for (int i = 0; i < count; i++)
            {
                T task = workingQueue.Dequeue();
                if (task.Status == TaskStatus.Done || task.Status == TaskStatus.Canceled || task.Status == TaskStatus.Error)
                {
                    ReferencePool.Recyle(task);
                    taskDic.Remove(task.ID);
                }
                else if (task.Status == TaskStatus.Paused)
                {
                    workingQueue.Enqueue(task);
                }
                else
                {
                    task.OnUpdate();
                    workingQueue.Enqueue(task);
                }
            }
        }

        // Task Events:
        public void AddTaskCancelEvent(int id, Action<TaskInfo> onCancel)
        {
            if (taskDic.ContainsKey(id))
            {
                taskDic[id].CancelEvent += onCancel;
            }
        }

        public void AddTaskCompleteEvent(int id, Action<TaskInfo> onCompleted)
        {
            if (taskDic.ContainsKey(id))
            {
                taskDic[id].CompleteEvent += onCompleted;
            }
        }

        public void AddTaskPauseEvent(int id, Action<TaskInfo> onPause)
        {
            if (taskDic.ContainsKey(id))
            {
                taskDic[id].PauseEvent += onPause;
            }
        }

        public void AddTaskResumeEvent(int id, Action<TaskInfo> onResume)
        {
            if (taskDic.ContainsKey(id))
            {
                taskDic[id].ResumeEvent += onResume;
            }
        }
    }
}
