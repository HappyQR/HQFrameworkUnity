using System.Threading.Tasks;

namespace HQFramework.Editor
{
    public interface IAssetUploader
    {
        Task UploadFileAsync(string filePath);
    }
}
