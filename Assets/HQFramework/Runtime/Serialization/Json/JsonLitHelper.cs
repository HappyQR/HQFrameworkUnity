using LitJson;

namespace HQFramework.Runtime
{
    public class JsonLitHelper : IJsonHelper
    {
        public string ToJson(object obj)
        {
            return JsonMapper.ToJson(obj);
        }

        public T ToObject<T>(string json)
        {
            return JsonMapper.ToObject<T>(json);
        }

        public object ToObject(string json)
        {
            return JsonMapper.ToObject(json);
        }
    }
}
