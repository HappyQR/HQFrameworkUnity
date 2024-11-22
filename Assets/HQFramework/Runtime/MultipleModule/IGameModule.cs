namespace HQFramework.Runtime
{
    public interface IGameModule
    {
        void OnModuleLoaded();

        void OnModuleEnter(object userData);

        void OnModuleUpdate();

        void OnModuleExit();
    }
}
