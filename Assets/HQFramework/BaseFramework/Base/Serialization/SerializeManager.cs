namespace HQFramework
{
    public sealed class SerializeManager
    {
        private static IBinaryHelper binaryHelper;
        private static IJsonHelper jsonHelper;
        private static IXmlHelper xmlHelper;

        public static void SetBinaryHelper(IBinaryHelper helper) => binaryHelper = helper;

        public static void SetJsonHelper(IJsonHelper helper) => jsonHelper = helper;

        public static void SetXmlHelper(IXmlHelper helper) => xmlHelper = helper;

        public static string ObjectToJson(object obj) => jsonHelper.ToJson(obj);

        public static T JsonToObject<T>(string json) => jsonHelper.ToObject<T>(json);

        public static object JsonToObject(string json) => jsonHelper.ToObject(json);

        public static byte[] ObjectToBytes(object obj) => binaryHelper.ToBytes(obj);

        public static T BytesToObject<T>(byte[] bytes) => binaryHelper.ToObject<T>(bytes);

        public static object BytesToObject(byte[] bytes) => binaryHelper.ToObject(bytes);

        public static string ObjectToXml(object obj) => xmlHelper.ToXml(obj);

        public static T XmlToObject<T>(string xml) => xmlHelper.ToObject<T>(xml);

        public static object XmlToObject(string xml) => xmlHelper.ToObject(xml);
    }
}
