using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HQFramework.Utility
{
    public static class HashUtility
    {
        public static string ComputeHash(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found : {filePath}");
            }
            using MD5 md5 = MD5.Create();
            using FileStream fs = File.OpenRead(filePath);
            byte[] bytes = md5.ComputeHash(fs);
            StringBuilder hash = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public static string ComputeHash(Stream stream)
        {
            using MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(stream);
            StringBuilder hash = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public static string ComputeHash(byte[] data)
        {
            using MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(data);
            StringBuilder hash = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public static string ConvertByHashBytes(byte[] hashBytes)
        {
            StringBuilder hash = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hash.Append(hashBytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
