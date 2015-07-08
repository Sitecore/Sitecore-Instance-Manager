using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Products.ModulesDetector.Core.Implementation.Rules
{
    public class EmptyRule : BaseRule
    {
        public override bool Execute(Abstraction.IModule module, Abstraction.IInstanceContext context)
        {
            return true;
        }
    }
}
