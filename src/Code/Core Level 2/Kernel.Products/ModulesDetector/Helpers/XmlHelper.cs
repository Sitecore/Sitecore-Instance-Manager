using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SIM.Products.ModulesDetector.Helpers
{
    public static class XmlHelper
    {
        public static string GetAttrValue(XmlNode node, string attrName)
        {
            if (node != null && attrName != null && attrName != "" && node.Attributes[attrName] != null)
            {
                return node.Attributes[attrName].Value;
            }
            else
            {
                return "";
            }
        }

        public static XmlDocument GetXmlDocument(string pathToFile)
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists(pathToFile))
            {
                doc.Load(pathToFile);
            }
            return doc;
        }

        public static Dictionary<string, string> ConvertXmlAttrsToDictionary(XmlAttributeCollection attributes)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (XmlAttribute attr in attributes)
            {
                result.Add(attr.Name, attr.Value);
            }
            return result;
        }
    }
}
