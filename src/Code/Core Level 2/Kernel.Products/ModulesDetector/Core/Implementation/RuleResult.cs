using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Products.ModulesDetector.Core.Implementation
{
    public class RuleResult
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public RuleResult(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public RuleResult(object value)
        {
            this.Name   = "name";
            this.Value  = value;
        }
    }
}
