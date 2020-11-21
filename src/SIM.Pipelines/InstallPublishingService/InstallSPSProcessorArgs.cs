using SIM.Pipelines.Processors;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class InstallSPSProcessorArgs : ProcessorArgs
  {
    #region Properties
    //Populated from InstallPublishingServiceWizardArgs
    public string InstanceName { get; set; }
    public string InstanceFolder { get; set; }
    public string SPSInstanceFolder { get; set; }
    public string SqlAdminUsername { get; set; }
    public string SqlAdminPassword { get; set; }
    public string SPSPackagePath { get; set; }
    public string SPSSiteName { get; set; }
    public int SPSPort { get; set; }
    public Dictionary<string, SqlConnectionStringBuilder> SPSConnectionStrings { get; set; }
    public bool OverwriteExisting { get; set; }

    //Other Properties
    public string SPSWebroot { get { return Path.Combine(SPSInstanceFolder, SPSSiteName); } }

    #endregion
  }
}
