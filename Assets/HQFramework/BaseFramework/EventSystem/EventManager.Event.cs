using System;
using System.Collections.Generic;

namespace HQFramework.EventSystem
{
    internal partial class EventManager
    {
        private class Event : IReference
        {
            private object sender;
            private EventArgsBase args;
            private LinkedList<EventHandler<EventArgsBase>> eventList;

            public static Event Create(object sender, EventArgsBase args, LinkedList<EventHandler<EventArgsBase>> eventList)
            {
                Event @event = ReferencePool.Spawn<Event>();
                @event.sender = sender;
                @event.args = args;
                @event.eventList = eventList;
                return @event;
            }

            public void Invoke()
            {
                for (LinkedListNode<EventHandler<EventArgsBase>> eventNode = eventList.First; eventNode != null; eventNode = eventNode.Next)
                {
                    eventNode.Value.Invoke(sender, args);
                }
            }
            
            void IReference.OnRecyle()
            {
                sender = null;
                ReferencePool.Recyle(args);
                eventList = null;
            }
        }
    }
}
