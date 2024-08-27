namespace HQFramework.Resource
{
    public interface IResourceManager
    {
        void SetHelper(IResourceHelper resourceHelper);

        void DecompressBuiltinResource();
    }
}
