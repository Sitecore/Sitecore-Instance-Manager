using SIM.Pipelines.Processors;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class InstallPublishingServiceProcessorArgs : ProcessorArgs
  {
    #region Properties
    public string InstanceName { get; set; }
    public string InstanceInstallRoot { get; set; }
    public string PublishingServiceInstallRoot { get; set; }
    public string SqlAdminUsername { get; set; }
    public string SqlAdminPassword { get; set; }
    public string PublishingServicePackagePath { get; set; }
    public string PublishingServiceSiteName { get; set; }
    public Dictionary<string, SqlConnectionStringBuilder> PublishingServiceConnectionStrings { get; set; }

    #endregion
  }
}
