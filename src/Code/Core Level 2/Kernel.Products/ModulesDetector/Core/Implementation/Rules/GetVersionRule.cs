using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SIM.Products.ModulesDetector.Core.Implementation.Rules
{
    class GetVersionRule : BaseRule
    {
        Dictionary<string, string> args = new Dictionary<string, string>();

        public GetVersionRule(Dictionary<string, string> args)
        {
            this.args = args;
        }
        public override bool Execute(Abstraction.IModule module, Abstraction.IInstanceContext context)
        {
            bool result = true;
            foreach (var file in Directory.GetFiles(Path.Combine(context.GetPathToWebsiteFolder(), "bin"), "*.*", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file).ToLower();
                var argsFileName = Path.GetFileName(args["file"]).ToLower();
                if (fileName == argsFileName)
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(file);
                    module.Version = fileVersionInfo.ProductVersion;
                }
            }
            return result;
        }
    }
}
