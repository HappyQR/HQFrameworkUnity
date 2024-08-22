namespace HQFramework.Version
{
    public interface IVersionHelper
    {
        string RemoteVersionUrl
        {
            get;
        }
        
        VersionInfo GetLocalVersionInfo();
    }
}
