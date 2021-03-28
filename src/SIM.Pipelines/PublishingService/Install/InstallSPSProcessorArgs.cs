using System.Collections.Generic;
using System.Data.SqlClient;
using SIM.Instances;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.PublishingService.Install
{
  public class InstallSPSProcessorArgs : ProcessorArgs
  {
    #region Properties
    public Instance CMInstance { get; set; }
    public string InstanceFolder { get; set; }
    public string SPSInstanceFolder { get; set; }
    public string SqlAdminUsername { get; set; }
    public string SqlAdminPassword { get; set; }
    public string SPSPackagePath { get; set; }
    public string SPSSiteName { get; set; }
    public string SPSAppPoolName { get; set; }
    public string SPSWebroot { get; set; }
    public int SPSPort { get; set; }
    public Dictionary<string, SqlConnectionStringBuilder> SPSConnectionStrings { get; set; }

    #endregion
  }
}
