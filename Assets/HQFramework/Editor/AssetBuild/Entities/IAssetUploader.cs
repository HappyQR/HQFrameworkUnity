using System.Threading.Tasks;

namespace HQFramework.Editor
{
    public interface IAssetUploader
    {
        string UrlRoot
        {
            get;
        }

        string HotfixRootFolder
        {
            get;
            set;
        }

        Task<bool> UploadAssetAsync(string url, string filePath);
    }
}
