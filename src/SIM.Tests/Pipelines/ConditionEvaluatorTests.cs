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
        public void ConditionEvaluator_NoCondition_ReturnsTrue()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));

        }

        [TestMethod]
        public void ConditionEvaluator_EqualsWithSame_ReturnsTrue()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action condition='equals' conditionArg1='a' conditionArg2='a' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }


        [TestMethod]
        public void ConditionEvaluator_EqualsWithDifferent_False()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action condition='equals' conditionArg1='a' conditionArg2='b' />");
            Assert.IsFalse(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluator_EqualsWithMixedCaseAndMatchingElements_True()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action condition='Equals' conditionArg1='a' conditionArg2='a' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluator_NotEqualsWithDifferent_True()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action condition='not equals' conditionArg1='a' conditionArg2='b' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }


        [TestMethod]
        public void ConditionEvaluator_NotEqualsWithSame_False()
        {
            var evaluator = new ConditionEvaluator();
            var element = GetXmlElement("<action condition='not equals' conditionArg1='a' conditionArg2='a' />");
            Assert.IsFalse(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluator_VariableInFirstElementMatches_True()
        {
            var variables = new Dictionary<string, string> {{"{variable}", "a"}};
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='{variable}' conditionArg2='a' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluator_VariableInSecondMatchesElement_True()
        {
            var variables = new Dictionary<string, string> { { "{variable}", "a" } };
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='a' conditionArg2='{variable}' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluator_VariableInFirstElementDoesNotMatch_False()
        {
            var variables = new Dictionary<string, string> { { "{variable}", "a" } };
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='{variable}' conditionArg2='b' />");
            Assert.IsFalse(evaluator.ConditionIsTrueOrMissing(element));
        }

        [TestMethod]
        public void ConditionEvaluator_VariableInSecondMatchesDoesNotMatch_False()
        {
            var variables = new Dictionary<string, string> { { "{variable}", "a" } };
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='b' conditionArg2='{variable}' />");
            Assert.IsFalse(evaluator.ConditionIsTrueOrMissing(element));
        }


        [TestMethod]
        public void ConditionEvaluator_VariableInBothElementsMatch_True()
        {
            var variables = new Dictionary<string, string> { { "{variable1}", "a" } , {"{variable2}", "a" } };
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='{variable1}' conditionArg2='{variable2}' />");
            Assert.IsTrue(evaluator.ConditionIsTrueOrMissing(element));
        }


        [TestMethod]
        public void ConditionEvaluator_VariableInBothElementsMatch_False()
        {
            var variables = new Dictionary<string, string> { { "{variable1}", "a" }, { "{variable2}", "b" } };
            var evaluator = new ConditionEvaluator(variables);
            var element = GetXmlElement("<action condition='equals' conditionArg1='{variable1}' conditionArg2='{variable2}' />");
            Assert.IsFalse(evaluator.ConditionIsTrueOrMissing(element));
        }

        private static XmlElement GetXmlElement(string action)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(string.Format("<root>{0}</root>", action));
            var element = doc.GetElementsByTagName("action")[0] as XmlElement;
            return element;
        }
    }
}
