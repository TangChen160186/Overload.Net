using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OvCore.OvCore.Ecs.Components;

namespace OvCore.OvCore.Api
{
    public static class SerializeHelper
    {
        public static void Serialize<T>(T instance, string fileName)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using XmlWriter writer = new XmlTextWriter(fileName, Encoding.UTF8);
            serializer.WriteObject(writer, instance);
        }
        public static T? DeSerialize<T>(string fileName)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using XmlReader reader = new XmlTextReader(fileName, fs);
            return (T)serializer.ReadObject(reader)!;
        }
    }
}
