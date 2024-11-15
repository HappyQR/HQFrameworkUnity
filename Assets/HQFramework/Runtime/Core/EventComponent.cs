using System;
using HQFramework.EventSystem;

namespace HQFramework.Runtime
{
    public class EventComponent : BaseComponent
    {
        private IEventManager eventManager;

        private void Start()
        {
            eventManager = HQFrameworkEngine.GetModule<IEventManager>();
        }

        public void RegisterEventListener(int id, EventHandler<EventArgsBase> @event)
        {
            eventManager.RegisterEventListener(id, @event);
        }

        public void UnregisterEventListener(int id, EventHandler<EventArgsBase> @event)
        {
            eventManager.UnregisterEventListener(id, @event);
        }

        public void InvokeEvent(object sender, EventArgsBase args)
        {
            eventManager.InvokeEvent(sender, args);
        }

        public void InvokeEventImmediately(object sender, EventArgsBase args)
        {
            eventManager.InvokeEventImmediately(sender, args);
        }
    }
}
