using JetBrains.Annotations;
using SIM.IO;
using SIM.Pipelines.Install;
using SIM.Pipelines.Processors;
using SIM.Products;
using Sitecore.Diagnostics.Base;
using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.Install
{
  public class Install9Args : ProcessorArgs
  {    
   public Install9Args(Tasker tasker)
    {
      Assert.ArgumentNotNull(tasker, nameof(tasker));
      this.Tasker = tasker;
    }

    public bool ScriptsOnly { get; set; }
    public Tasker Tasker { get; }
    
  }
}
