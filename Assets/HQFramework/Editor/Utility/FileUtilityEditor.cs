using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;

namespace HQFramework.Editor
{
    public static class FileUtilityEditor
    {
        public static string GetPhysicalPath(string path)
        {
            return FileUtil.GetPhysicalPath(path);
        }

        public static string GetLogicalPath(string path)
        {
            return FileUtil.GetLogicalPath(path);
        }

        public static string GetMD5(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path} does not exist");
            }

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                StringBuilder hash = new StringBuilder();
                using MD5 md5 = MD5.Create();
                byte[] bytes = md5.ComputeHash(fs);
                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Append(bytes[i].ToString("x2"));
                }
                return hash.ToString();
            }
        }

        public static int GetFileSize(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path} does not exist");
            }

            FileInfo file = new FileInfo(path);
            return (int)file.Length;
        }
    }
}
