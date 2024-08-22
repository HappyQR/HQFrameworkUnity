namespace HQFramework
{
    public interface IXmlHelper
    {
        string ToXml(object obj);

        T ToObject<T>(string xml);

        object ToObject(string xml);
    }
}
