using System;

namespace HQFramework.EventSystem
{
    public interface IEventManager
    {
        void RegisterEventListener(int id, EventHandler<EventArgsBase> @event);

        void UnregisterEventListener(int id, EventHandler<EventArgsBase> @event);

        void InvokeEvent(object sender, EventArgsBase args);

        void InvokeEventImmediately(object sender, EventArgsBase args);
    }
}
