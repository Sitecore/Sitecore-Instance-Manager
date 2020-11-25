namespace SIM.Tool.Base.Pipelines
{
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using SIM.Pipelines.Install;
  using SIM.Pipelines.InstallPublishingService;
  using SIM.Pipelines.Processors;
  using SIM.Services;
  using SIM.Tool.Base.Converters;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  public class InstallSPSWizardArgs : WizardArgs
  {
    private string _spsPackagePath;
    public InstallSPSWizardArgs(Instance instance)
    {
      Initialize(instance);
    }

    #region Properties

    //From Instance
    public Instance Instance { get; private set; }
    public string InstanceName { get; private set; }
    public ConnectionStringCollection InstanceConnectionStrings { get; private set; }

    //From Profile
    public string InstanceFolder { get; set; }
    public string SPSInstanceFolder { get; set; }

    public string SqlAdminUsername { get; private set; }
    public string SqlAdminPassword { get; private set; }


    //From User Input
    public bool OverwriteExisting { get; set; }
    public string SPSPackage
    {
      get { return Path.GetFileNameWithoutExtension(_spsPackagePath); }
      set { _spsPackagePath = value; }
    }
    public string SPSSiteName { get; set; }
    public int SPSPort { get; set; }
    public Dictionary<string, SqlConnectionStringBuilder> SPSConnectionStrings { get; set; } = new Dictionary<string, SqlConnectionStringBuilder>();

    #endregion

    #region Methods
    private void Initialize(Instance instance)
    {
      this.Instance = instance;
      this.InstanceName = instance.Name;
      this.InstanceFolder = ProfileManager.Profile.InstancesFolder;
      this.InstanceConnectionStrings = instance.Configuration.ConnectionStrings;
      this.SPSInstanceFolder = ProfileManager.Profile.InstancesFolder;

      SqlConnectionStringBuilder SqlServerConnectionString = ProfileManager.GetConnectionString();
      this.SqlAdminUsername = SqlServerConnectionString.UserID;
      this.SqlAdminPassword = SqlServerConnectionString.Password;
    }

    public override ProcessorArgs ToProcessorArgs()
    {
      return new InstallSPSProcessorArgs()
      {
        InstanceName = this.InstanceName,
        SPSSiteName = this.SPSSiteName,
        SPSAppPoolName = this.SPSSiteName,
        SPSWebroot = Path.Combine(this.SPSInstanceFolder, this.SPSSiteName),
        SPSPort = this.SPSPort,
        InstanceFolder = this.InstanceFolder,
        SPSInstanceFolder = this.SPSInstanceFolder,
        SPSPackagePath = this._spsPackagePath,
        SqlAdminUsername = this.SqlAdminUsername,
        SqlAdminPassword = this.SqlAdminPassword,
        SPSConnectionStrings = this.SPSConnectionStrings,
        OverwriteExisting = this.OverwriteExisting
      };
    }

    #endregion
  }
}