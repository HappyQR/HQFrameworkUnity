using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace HQFramework.Editor
{
    public static partial class AssetUtility
    {
        public static class ConfigSerializer
        {
            public static byte[] Serialize(object target)
            {
                string jsonStr = JsonUtilityEditor.ToJson(target);
                return Encoding.UTF8.GetBytes(jsonStr);
            }

            public static T Deserialize<T>(byte[] data)
            {
                string jsonStr = Encoding.UTF8.GetString(data);
                return JsonUtilityEditor.ToObject<T>(jsonStr);
            }

            public static bool SerializeToFile(object target, string path)
            {
                string jsonStr = JsonUtilityEditor.ToJson(target);
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                try
                {
                    File.WriteAllText(path, jsonStr);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                    return false;
                }
            }

            public static T DeserializeFromFile<T>(string path) where T : class
            {
                if (!File.Exists(path))
                {
                    return null;
                }

                string jsonStr = File.ReadAllText(path);
                T result = JsonUtilityEditor.ToObject<T>(jsonStr);
                return result;
            }
        }
    }
}
