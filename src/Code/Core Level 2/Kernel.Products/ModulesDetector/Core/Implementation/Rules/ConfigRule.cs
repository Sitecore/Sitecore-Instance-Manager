
using SIM.Products.ModulesDetector.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SIM.Products.ModulesDetector.Core.Implementation.Rules
{
    public class ConfigRule : BaseRule
    {
        Dictionary<string, string> args = new Dictionary<string, string>();

        public ConfigRule(Dictionary<string, string> args)
        {
            this.args = args;
        }

        public override bool Execute(IModule module, IInstanceContext context)
        {
            bool result = true;
            foreach (var file in Directory.GetFiles(Path.Combine(context.GetPathToWebsiteFolder(), "App_Config"), "*.*", SearchOption.AllDirectories))
            {
                var fileName     = Path.GetFileName(file).ToLower();
                var argsFileName = Path.GetFileName(args["file"]).ToLower();
                if (fileName == argsFileName)
                {
                    module.Status = ModuleStatus.Enabled;
                }
                else if ((fileName == argsFileName + ".disabled") || (fileName == argsFileName + ".example"))
                {
                    module.Status = ModuleStatus.Disabled;
                }
            }
            return result;

        }
    }
}
