namespace HQFramework.Runtime
{
    public interface IGameModuleManager
    {
        void LaunchModule(int moduleID);

        void KillModule(int moduleID);
    }
}
