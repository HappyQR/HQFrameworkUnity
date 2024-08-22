using LitJson;

namespace HQFramework.Editor
{
    public class JsonUtilityEditor
    {
        public static string ToJson(object obj)
        {
            return JsonMapper.ToJson(obj);
        }

        public static T ToObject<T>(string json)
        {
            return JsonMapper.ToObject<T>(json);
        }

        public static object ToObject(string json)
        {
            return JsonMapper.ToObject(json);
        }
    }
}
