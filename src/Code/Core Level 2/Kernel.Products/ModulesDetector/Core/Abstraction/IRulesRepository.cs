using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Products.ModulesDetector.Core.Abstraction
{
    public interface IRulesRepository
    {
        Dictionary<string, IRule> GetRules();
    }
}
