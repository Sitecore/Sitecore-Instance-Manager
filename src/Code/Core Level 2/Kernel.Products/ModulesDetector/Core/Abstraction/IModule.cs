
using SIM.Products.ModulesDetector.Core.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Products.ModulesDetector.Core.Abstraction
{
    public interface IModule
    {
        string Name             { get; set; }
        string Version          { get; set; }
        ModuleStatus Status     { get; set; }

        void SetVersion(string version);
        void SetStatus(ModuleStatus status);
    }
}
