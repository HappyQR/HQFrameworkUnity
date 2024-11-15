using System;
using System.Collections.Generic;

namespace HQFramework.EventSystem
{
    internal sealed partial class EventManager : HQModuleBase, IEventManager
    {
        public override byte Priority => 1;

        private Dictionary<int, LinkedList<EventHandler<EventArgsBase>>> eventDic;
        private Queue<Event> invokeQueue;

        protected override void OnInitialize()
        {
            eventDic = new Dictionary<int, LinkedList<EventHandler<EventArgsBase>>>();
            invokeQueue = new Queue<Event>();
        }

        protected override void OnUpdate()
        {
            while (invokeQueue.Count > 0)
            {
                Event @event = invokeQueue.Dequeue();
                @event.Invoke();
                ReferencePool.Recyle(@event);
            }
        }

        public void RegisterEventListener(int id, EventHandler<EventArgsBase> @event)
        {
            if (!eventDic.ContainsKey(id))
            {
                eventDic.Add(id, new LinkedList<EventHandler<EventArgsBase>>());
            }

            eventDic[id].AddLast(@event);
        }

        public void UnregisterEventListener(int id, EventHandler<EventArgsBase> @event)
        {
            if (eventDic.TryGetValue(id, out LinkedList<EventHandler<EventArgsBase>> eventList))
            {
                eventList.Remove(@event);
            }
            else
            {
                HQDebugger.LogWarning($"You have not registered {id} event.");
            }
        }

        public void InvokeEvent(object sender, EventArgsBase args)
        {
            if (eventDic.TryGetValue(args.SerialID, out LinkedList<EventHandler<EventArgsBase>> eventList))
            {
                Event @event = Event.Create(sender, args, eventList);
                invokeQueue.Enqueue(@event);
            }
            else
            {
                HQDebugger.LogWarning($"You have not registered {args.SerialID} event.");
            }
        }

        public void InvokeEventImmediately(object sender, EventArgsBase args)
        {
            if (eventDic.TryGetValue(args.SerialID, out LinkedList<EventHandler<EventArgsBase>> eventList))
            {
                Event @event = Event.Create(sender, args, eventList);
                @event.Invoke();
                ReferencePool.Recyle(@event);
            }
            else
            {
                HQDebugger.LogWarning($"You have not registered {args.SerialID} event.");
            }
        }

        protected override void OnShutdown()
        {
            eventDic.Clear();
            while (invokeQueue.Count > 0)
            {
                ReferencePool.Recyle(invokeQueue.Dequeue());
            }
        }
    }
}
