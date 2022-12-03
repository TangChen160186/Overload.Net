using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OvCore.OvCore.Api
{
    public interface ISerializable
    {
        void OnSerialize(XmlDocument doc, XmlNode node);
        void OnDeserialize(XmlDocument doc, XmlNode node);
    }
}
