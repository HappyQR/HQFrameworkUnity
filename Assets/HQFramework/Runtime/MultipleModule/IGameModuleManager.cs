namespace HQFramework.Runtime
{
    public interface IGameModuleManager
    {
        void LaunchModule(int moduleID, object userData);

        void KillModule(int moduleID);
    }
}
