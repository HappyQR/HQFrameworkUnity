using System.Text;

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
        }
    }
}
