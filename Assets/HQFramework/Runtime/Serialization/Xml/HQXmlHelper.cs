using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class HQXmlHelper : IXmlHelper
    {
        public static string SAVE_PATH
        {
            get
            {
                return Application.persistentDataPath + "/User/";
            }
        }

        public void SaveData(string fileName, object data)
        {
            if (!Directory.Exists(SAVE_PATH))
            {
                Directory.CreateDirectory(SAVE_PATH);
            }
            fileName += ".xml";
            using (StreamWriter writer = new StreamWriter(SAVE_PATH + fileName))
            {
                XmlSerializer xs = new XmlSerializer(data.GetType());
                xs.Serialize(writer, data);
            }
        }

        public T LoadData<T>(string fileName) where T : class
        {
            fileName += ".xml";

            if (!File.Exists(SAVE_PATH + fileName))
            {
                return default;
            }
            using (StreamReader reader = new StreamReader(SAVE_PATH + fileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                return xs.Deserialize(reader) as T;
            }
        }

        public string ToXml(object obj)
        {
            return null;
        }

        public T ToObject<T>(string xml)
        {
            throw new NotImplementedException();
        }

        public object ToObject(string xml)
        {
            throw new NotImplementedException();
        }
    }

}