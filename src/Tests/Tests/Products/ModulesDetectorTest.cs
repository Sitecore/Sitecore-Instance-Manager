using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIM.Products.ModulesDetector.Core.Abstraction;
using SIM.Products.ModulesDetector.Core.Implementation;
using SIM.Products.ModulesDetector.Core.Implementation.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SIM.Tests.Products
{

    public class FakeInstanceContext : IInstanceContext
    {
        public string GetPathToWebsiteFolder()
        {
            //return @"D:\inetpub\wwwRoot\sc80rev141109test\Website";
            return @"D:\inetpub\wwwRoot\sc650rev120706br\Website";
        }
    }

    public class FakeRulesRepo : IRulesRepository
    {

        public Dictionary<string, IRule> GetRules()
        {

            Dictionary<string, string> ruleArgs = new Dictionary<string, string>()
            {
                {"file","Sitecore.Video.Brightcove.config"}
            };

            Dictionary<string, IRule> result = new Dictionary<string, IRule>() 
            { 
                {"test1", new EmptyRule()},
                {"test2", new EmptyRule()},
                {"test3", new EmptyRule()},
                {"test4", new EmptyRule()},
            };

            return result;
        }
    }

    [TestClass]
    public class ModulesDetectorTest
    {
        [TestInitialize]
        public void Initialize()
        {
            TestHelper.Initialize();               
        }

        [TestMethod]
        public void ModulesDetectTest()
        {
            IInstanceContext context = new FakeInstanceContext();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<manifest version=\"1.3\"><package><name>Brightcove Video Cloud Connect</name></package><detect><rule ruleType=\"CheckFileExist\" file=\"Website/App_Config/Include/Sitecore.Video.Brightcove.config\"><rule ruleType=\"GetVersionRule\" file=\"Sitecore.Video.Brightcove.dll\" /></rule><rule ruleType=\"CheckFileExist\" file=\"Website/App_Config/Include/Sitecore.Video.Brightcove.config\"><rule ruleType=\"GetVersionRule\" file=\"Sitecore.Video.Brightcove.dll\" /></rule></detect></manifest>");
            IRulesRepository rulesRepo = new XmlFileRulesRepository(doc);

            ModulesDetector detector = new ModulesDetector(rulesRepo);

            var results = detector.CollectResults(context);
            List<string> actual = new List<string>();

            foreach (var module in results)
            {
                actual.Add(module.Name + ":" + module.Status + ":" + module.Version.ToString());
            }

            List<string> expected = new List<string>();
            expected.Add("Brightcove Video Cloud Connect:Enabled:6.5.0 rev 130517");

            Assert.IsTrue(actual.SequenceEqual(expected), String.Format("Actual:{0} Expected:{1}", actual[0], expected[0]));
        }

        [TestMethod]
        public void ModulesDetectXmlRulesRepoTest()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<manifest version=\"1.3\"><package><name>Brightcove Video Cloud Connect</name></package><detect><rule ruleType=\"CheckFileExist\" file=\"Website/App_Config/Include/Sitecore.Video.Brightcove.config\"><rule ruleType=\"GetVersionRule\" file=\"Sitecore.Video.Brightcove.dll\" /></rule><rule ruleType=\"CheckFileExist\" file=\"Website/App_Config/Include/Sitecore.Video.Brightcove.config\"><rule ruleType=\"GetVersionRule\" file=\"Sitecore.Video.Brightcove.dll\" /></rule></detect></manifest>");
            IRulesRepository rulesRepo = new XmlFileRulesRepository(doc);

            var results = rulesRepo.GetRules();

            Assert.IsTrue(results.Keys.First() == "Brightcove Video Cloud Connect");
            Assert.IsTrue(results.Values.All(x => x.ChildRules.Count == 2));
            Assert.IsTrue(results.Values.All(x => x.ChildRules.All(y => y.ChildRules.Count == 1)));
        }
    }
    
}
