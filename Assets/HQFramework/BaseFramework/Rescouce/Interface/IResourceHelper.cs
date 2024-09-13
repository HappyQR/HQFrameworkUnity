namespace HQFramework.Resource
{
    public interface IResourceHelper
    {
        int HotfixDownloadGroupID
        {
            get;
        }
        
        ResourceConfig LoadResourceConfig();
    }
}
