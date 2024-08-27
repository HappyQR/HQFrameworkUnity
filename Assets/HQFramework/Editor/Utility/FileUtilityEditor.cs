using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

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

        public static string GetRelativePath(string fromPath, string toPath)
        {
            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);

            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                relativePath = relativePath.Replace('/', '\\');
            }

            return relativePath;
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
