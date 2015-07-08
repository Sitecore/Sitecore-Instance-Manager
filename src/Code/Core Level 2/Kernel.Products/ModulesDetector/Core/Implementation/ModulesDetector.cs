
using SIM.Products.ModulesDetector.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Products.ModulesDetector.Core.Implementation
{
    public class ModulesDetector
    {
        IRulesRepository    rulesRepo;
        public ModulesDetector(IRulesRepository rulesRepository)
        {
            rulesRepo       = rulesRepository;
        }

        public List<Module> CollectResults(IInstanceContext context)
        {
            var undetectedModules = rulesRepo.GetRules();
            List<Module> result = new List<Module>();

            foreach (var moduleName in undetectedModules.Keys)
            {
                IModule module = new Module(moduleName);
                IRule rule = undetectedModules[moduleName];

                ProcessRule(rule, module, context);//recursion

                if (module.Status != ModuleStatus.Undetected) result.Add((Module)module);
            }

            return result;
        }

        void ProcessRule(IRule rule, IModule module, IInstanceContext context)//recursion
        {
            bool ruleresult = rule.Execute(module, context);
            if (ruleresult == true)
            {
                foreach (IRule childRule in rule.ChildRules)
                {
                    ProcessRule(childRule, module, context);
                }
            }
        }
    }
}
