using System.Collections.Generic;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIM.Pipelines;

namespace SIM.Tests.Pipelines
{
    [TestClass]
    public class ConditionEvaluatorTests
    {
        [TestMethod]
        public void ConditionEvaluatorNoConditionReturnsTrue()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));

        }

        [TestMethod]
        public void ConditionEvaluatorEqualsWithSameReturnsTrue()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action condition='equals' conditionArg1='a' conditionArg2='a' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }


        [TestMethod]
        public void ConditionEvaluatorEqualsWithDifferentFalse()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action condition='equals' conditionArg1='a' conditionArg2='b' />");
            Assert.IsFalse(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluatorEqualsWithMixedCaseAndMatchingElementsTrue()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action condition='Equals' conditionArg1='a' conditionArg2='a' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluatorNotEqualsWithDifferentTrue()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action condition='not equals' conditionArg1='a' conditionArg2='b' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }


        [TestMethod]
        public void ConditionEvaluatorNotEqualsWithSameFalse()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action condition='not equals' conditionArg1='a' conditionArg2='a' />");
            Assert.IsFalse(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluatorVariableInFirstElementMatchesTrue()
        {
            var variables = new Dictionary<string, string> {{"{variable}", "a"}};
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='{variable}' conditionArg2='a' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluatorVariableInSecondMatchesElementTrue()
        {
            var variables = new Dictionary<string, string> { { "{variable}", "a" } };
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='a' conditionArg2='{variable}' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluatorVariableInFirstElementDoesNotMatchFalse()
        {
            var variables = new Dictionary<string, string> { { "{variable}", "a" } };
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='{variable}' conditionArg2='b' />");
            Assert.IsFalse(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluatorVariableInSecondMatchesDoesNotMatchFalse()
        {
            var variables = new Dictionary<string, string> { { "{variable}", "a" } };
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='b' conditionArg2='{variable}' />");
            Assert.IsFalse(evaluator.ConditionIsTrueOrMissing(element));
        }


        [TestMethod]
        public void ConditionEvaluatorVariableInBothElementsMatchTrue()
        {
            var variables = new Dictionary<string, string> { { "{variable1}", "a" } , {"{variable2}", "a" } };
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='{variable1}' conditionArg2='{variable2}' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }


        [TestMethod]
        public void ConditionEvaluatorVariableInBothElementsMatchFalse()
        {
            var variables = new Dictionary<string, string> { { "{variable1}", "a" }, { "{variable2}", "b" } };
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='{variable1}' conditionArg2='{variable2}' />");
            Assert.IsFalse(evaluator.ConditionIsTrueOrMissing(element));
        }

        private static XmlElement GetXmlElement(string action)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml($"<root>{action}</root>");
            var element = doc.GetElementsByTagName("action")[0] as XmlElement;
            return element;
        }
    }
}
