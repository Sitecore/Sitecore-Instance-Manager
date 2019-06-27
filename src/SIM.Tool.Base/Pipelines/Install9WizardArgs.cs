using SIM.Adapters.SqlServer;
using SIM.Instances;
using SIM.IO;
using SIM.Pipelines;
using SIM.Pipelines.Install;
using SIM.Pipelines.Processors;
using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Tool.Base.Pipelines
{
  public class Install9WizardArgs : InstallWizardArgs
  {
    public Install9WizardArgs() : base()
    {

    }

    public Install9WizardArgs(Instance instance) : base(instance)
    {

    }

    public override ProcessorArgs ToProcessorArgs()
    {
      Install9Args args= new Install9Args(this.Tasker);
      args.ScriptsOnly = this.ScriptsOnly;
      return args;
    }
    public string SolrUrl { get; set; }
    public string SorlRoot { get; set; }
    public string ScriptRoot { get; set; }
    public Tasker Tasker { get; set; }
    public bool ScriptsOnly { get; set; }
  }
}
