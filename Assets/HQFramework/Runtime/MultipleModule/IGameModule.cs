namespace HQFramework.Runtime
{
    public interface IGameModule
    {
        void OnModuleLoaded();

        void OnModuleEnter();

        void OnModuleUpdate();

        void OnModuleExit();
    }
}
