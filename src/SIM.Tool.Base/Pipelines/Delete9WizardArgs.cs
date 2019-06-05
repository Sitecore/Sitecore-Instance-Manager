using SIM.Instances;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Tool.Base.Pipelines
{
  public class Delete9WizardArgs:Install9WizardArgs
  {    
    public Delete9WizardArgs(Instance instance, SqlConnectionStringBuilder connectionString, string uninstallPath)
    {
      this.InstanceName = instance.Name;
      this.InstanceConnectionString = connectionString;
      this.Product = instance.Product;
      this.UnInstallPath = uninstallPath;
    }

   public string UnInstallPath { get; }
  }
}
