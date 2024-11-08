using System.Threading.Tasks;
using HQFramework.Editor;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using UnityEngine;
using System;

namespace HQFramework.Sample
{
    public class AmazonAssetUploader : IAssetUploader
    {
        public string UrlRoot => "https://assets.moonvrhome.com";

        public string HotfixRootFolder
        {
            get;
            set;
        }
        
        public string HotfixManifestFileName
        {
            get;
            set;
        }

        private AmazonS3Client client;

        private static readonly string bucketName = "assets.moonvrhome.com";

        public AmazonAssetUploader()
        {
            string[] key_id = File.ReadAllText(Path.Combine(Application.dataPath, "../Build/Amazon.txt")).Split('|');
            string accessId = key_id[0];
            string accessKey = key_id[1];
            AmazonS3Config config = new AmazonS3Config();
            config.RegionEndpoint = RegionEndpoint.EUWest2;
            config.Timeout = TimeSpan.FromSeconds(10);
            client = new AmazonS3Client(accessId, accessKey, config);
        }

        public async Task<bool> UploadAssetAsync(string relatedUrl, string filePath)
        {
            byte[] data = await File.ReadAllBytesAsync(filePath);
            return await UploadAssetAsync(relatedUrl, data);
        }

        public async Task<bool> UploadAssetAsync(string relatedUrl, byte[] content)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = bucketName;
            request.Key = Path.Combine(HotfixRootFolder, relatedUrl);
            request.InputStream = new MemoryStream(content);
            try
            {
                PutObjectResponse response = await client.PutObjectAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }

        public void Dispose()
        {
            client.Dispose();
            client = null;
        }
    }
}
