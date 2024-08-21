namespace HQFramework
{
    internal abstract class HQModuleBase
    {
        public abstract byte Priority
        { 
            get; 
        }

        protected virtual void OnInitialize()
        {

        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void OnShutdown()
        {

        }
    }
}
