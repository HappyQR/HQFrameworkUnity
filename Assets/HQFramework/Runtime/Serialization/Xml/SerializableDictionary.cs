using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HQFramework.Runtime
{
    /// <summary>
    /// 继承自Dictionary，支持序列化和反序列化的字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 自定义字典的反序列化规则
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            //key的翻译机
            XmlSerializer keyXs = new XmlSerializer(typeof(TKey));
            //value的翻译机
            XmlSerializer valueXs = new XmlSerializer(typeof(TValue));
            //跳过根节点
            reader.Read();
            //判断当前不是元素节点结束就进行反序列化
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                //反序列化键
                TKey key = (TKey)keyXs.Deserialize(reader);
                //反序列化值
                TValue value = (TValue)valueXs.Deserialize(reader);
                //存到字典中
                Add(key, value);
            }
            //读取结束节点
            reader.Read();
        }

        /// <summary>
        /// 自定义字典的序列化规则
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            //key的翻译机
            XmlSerializer keyXs = new XmlSerializer(typeof(TKey));
            //value的翻译机
            XmlSerializer valueXs = new XmlSerializer(typeof(TValue));

            foreach (var item in this)
            {
                keyXs.Serialize(writer, item.Key);
                valueXs.Serialize(writer, item.Value);
            }
        }
    }
}
