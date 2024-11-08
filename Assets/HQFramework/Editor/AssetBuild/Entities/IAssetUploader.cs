using System;
using System.Threading.Tasks;

namespace HQFramework.Editor
{
    public interface IAssetUploader : IDisposable
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

        string HotfixManifestFileName
        {
            get;
            set;
        }

        Task<bool> UploadAssetAsync(string relatedUrl, string filePath);

        Task<bool> UploadAssetAsync(string relatedUrl, byte[] content);
    }
}
