namespace HQFramework.Resource
{
    public interface IResourceManager
    {
        ResourceConfig Config
        {
            get;
        }

        void SetHelper(IResourceHelper resourceHelper);

        // int LoadAsset(int crc);
    }
}
