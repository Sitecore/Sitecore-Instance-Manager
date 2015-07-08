
using SIM.Products.ModulesDetector.Core.Abstraction;
using SIM.Products.ModulesDetector.Core.Implementation.Rules;
using SIM.Products.ModulesDetector.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SIM.Products.ModulesDetector.Core.Implementation
{
    //TODO: read all nodes, add childs parameter for rule, set the structure like this: Dictionary<string, List<IRule>> 
    public class XmlFileRulesRepository : IRulesRepository
    {
        Dictionary<string, XmlNode> unparsedRules = new Dictionary<string, XmlNode>();
        List<IRule> rules = new List<IRule>();
        string rootRuleNodePath = @"/manifest/detect";
        string moduleNameNodePath = @"/manifest/package/name";

        public XmlFileRulesRepository(string folderPath)
        {
            foreach (var file in Directory.GetFiles(folderPath, "*.xml"))
            {
                var doc = XmlHelper.GetXmlDocument(file);
                var rootRuleNode = doc.SelectSingleNode(rootRuleNodePath);
                string moduleName;
                var moduleNameNode = doc.SelectSingleNode(moduleNameNodePath);

                if (moduleNameNode == null) moduleName = Path.GetFileNameWithoutExtension(file);
                else moduleName = moduleNameNode.InnerText;
                unparsedRules.Add(moduleName, rootRuleNode);
            }

        }

        public XmlFileRulesRepository(XmlDocument doc)
        {
            var rootRuleNode = doc.SelectSingleNode(rootRuleNodePath);
            string moduleName;
            var moduleNameNode = doc.SelectSingleNode(moduleNameNodePath);

            if (moduleNameNode == null) moduleName = "Unnamed";
            else moduleName = moduleNameNode.InnerText;
            unparsedRules.Add(moduleName, rootRuleNode);
        }

        public IRule ParseRule(XmlNode ruleNode)
        { 
            //TODO: maybe instansiate type from the "ruleTypeAttr" using Reflection
            IRule result = new EmptyRule();
            if (ruleNode != null && ruleNode.Attributes != null && ruleNode.Attributes["ruleType"] != null)
            {
                switch (ruleNode.Attributes["ruleType"].Value)
                {
                    case "CheckFileExist":
                        result = new ConfigRule(XmlHelper.ConvertXmlAttrsToDictionary(ruleNode.Attributes));
                        break;
                    case "GetVersionRule":
                        result = new GetVersionRule(XmlHelper.ConvertXmlAttrsToDictionary(ruleNode.Attributes));
                        break;
                    default:
                        //throw new Exception("Can't parse rule.");
                        return null;
                }
            }
            return result;
        }

        public IRule ParseRules(XmlNode ruleNode)
        {
            IRule result = ParseRule(ruleNode);//root
             if (ruleNode.ChildNodes != null)
            {
                foreach (XmlNode node in ruleNode.ChildNodes)
                {
                    result.ChildRules.Add(ParseRules(node));
                }
            }
            return result;
        }        

        public Dictionary<string, IRule> GetRules()
        {
            Dictionary<string, IRule> result = new Dictionary<string, IRule>();
            foreach (var key in unparsedRules.Keys)
            {
                if (unparsedRules[key] != null && ParseRules(unparsedRules[key]) != null)
                    result[key] = ParseRules(unparsedRules[key]);
                    
            }
            return result;
        }
    }
}
