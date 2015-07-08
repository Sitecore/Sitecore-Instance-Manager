
using SIM.Products.ModulesDetector.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Products.ModulesDetector.Core.Implementation
{
    public class Module : IModule
    {
        public string Name { get; set; }

        public string Version       { get; set; }
        public ModuleStatus Status  { get; set; }

        public Module(string name)
        {
            Name = name;
            Status = ModuleStatus.Undetected;
            Version = "";
        }


        public void SetVersion(string version)
        {
            Version = version;
        }

        public void SetStatus(ModuleStatus status)
        {
            switch (status)
            { 
                case ModuleStatus.Disabled:
                        if (this.Status != ModuleStatus.Enabled) this.Status = status;
                    break;
                case ModuleStatus.Enabled:
                    this.Status = status;
                        break;
                case ModuleStatus.Corrupted:
                        this.Status = status;
                    break;
            }
        }
    }
}
