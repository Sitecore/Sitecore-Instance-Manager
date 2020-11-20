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
  public class InstallPublishingServiceProcessorArgs : ProcessorArgs
  {
    #region Properties
    //Populated from InstallPublishingServiceWizardArgs
    public string InstanceName { get; set; }
    public string InstanceFolder { get; set; }
    public string PublishingServiceInstanceFolder { get; set; }
    public string SqlAdminUsername { get; set; }
    public string SqlAdminPassword { get; set; }
    public string PublishingServicePackagePath { get; set; }
    public string PublishingServiceSiteName { get; set; }
    public Dictionary<string, SqlConnectionStringBuilder> PublishingServiceConnectionStrings { get; set; }
    public bool OverwriteExisting { get; set; }

    //Other Properties
    public string PublishingServiceWebroot { get { return Path.Combine(PublishingServiceInstanceFolder, PublishingServiceSiteName); } }

    #endregion
  }
}
