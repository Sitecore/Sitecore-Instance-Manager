using SIM.Products.ModulesDetector.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Products.ModulesDetector.Core.Implementation.Rules
{
    public abstract class BaseRule : IRule
    {

        public BaseRule()
        {
            ChildRules = new List<IRule>();
        }

        public List<IRule> ChildRules
        {
            get;
            set;
        }

        public abstract bool Execute(IModule module, IInstanceContext context);
        
    }
}
