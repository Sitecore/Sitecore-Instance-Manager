using System.Collections.Generic;
using System.Xml;

namespace SIM.Pipelines
{
    /// <summary>
    /// Used to evaluate conditions in Manifest actions, following this syntax:
    ///  &lt;action condition="equals|not equals" conditionArg1="{Variable} conditionArg2="value"&gt;
    /// </summary>
    public class ConditionEvaluator
    {
        private readonly Dictionary<string, string> _variables;

        public ConditionEvaluator(Dictionary<string, string> variables = null)
        {
            if (variables == null)
            {
                variables = new Dictionary<string, string>();
            }
            _variables = variables;
        }

        public bool ConditionIsTrueOrMissing(XmlElement element)
        {
            var condition = element.Attributes["condition"];
            var arg1 = element.Attributes["conditionArg1"];
            var arg2 = element.Attributes["conditionArg2"];

            if (condition == null || arg1 == null || arg2 == null) return true;

            var isEquals = condition.Value.ToLower().Equals("equals");


            string arg1Value = arg1.Value;
            string arg2Value = arg2.Value;

            foreach (KeyValuePair<string, string> valuePair in _variables)
            {
                if (arg1Value == valuePair.Key)
                {
                    arg1Value = valuePair.Value;
                }
                if (arg2Value == valuePair.Key)
                {
                    arg2Value = valuePair.Value;
                }
            }

            var elementsMatch = arg1Value == arg2Value;

            return isEquals ? elementsMatch : !elementsMatch;
        }
    }
}