namespace HQFramework
{
    public interface IJsonHelper
    {
        string ToJson(object obj);

        T ToObject<T>(string json);

        object ToObject(string json);
    }
}
