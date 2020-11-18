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

  public class InstallPublishingServiceWizardArgs : WizardArgs
  {
    private string _publishingServicePackagePath;
    public InstallPublishingServiceWizardArgs(Instance instance)
    {
      Initialize(instance);
    }

    #region Properties

    //From Instance
    public Instance Instance { get; private set; }
    public string InstanceName { get; private set; }
    public ConnectionStringCollection InstanceConnectionStrings { get; private set; }

    //From Profile
    public string InstanceInstallRoot { get; set; }
    public string PublishingServiceInstallRoot { get; set; }

    public string SqlAdminUsername { get; private set; }
    public string SqlAdminPassword { get; private set; }


    //From User Input
    public string PublishingServicePackage
    {
      get { return Path.GetFileNameWithoutExtension(_publishingServicePackagePath); }
      set { _publishingServicePackagePath = value; }
    }
    public string PublishingServiceSiteName { get; set; }
    public Dictionary<string, SqlConnectionStringBuilder> PublishingServiceConnectionStrings { get; set; }

    #endregion

    #region Methods
    private void Initialize(Instance instance)
    {
      this.Instance = instance;
      this.InstanceName = instance.Name;
      this.InstanceInstallRoot = ProfileManager.Profile.InstancesFolder;
      this.InstanceConnectionStrings = instance.Configuration.ConnectionStrings;
      this.PublishingServiceInstallRoot = ProfileManager.Profile.InstancesFolder;

      SqlConnectionStringBuilder SqlServerConnectionString = ProfileManager.GetConnectionString();
      this.SqlAdminUsername = SqlServerConnectionString.UserID;
      this.SqlAdminPassword = SqlServerConnectionString.Password;
    }

    public override ProcessorArgs ToProcessorArgs()
    {
      return new InstallPublishingServiceProcessorArgs()
      {
        InstanceName = this.InstanceName,
        PublishingServiceSiteName = this.PublishingServiceSiteName,
        InstanceInstallRoot = this.InstanceInstallRoot,
        PublishingServiceInstallRoot = this.PublishingServiceInstallRoot,
        PublishingServicePackagePath = this._publishingServicePackagePath,
        SqlAdminUsername = this.SqlAdminUsername,
        SqlAdminPassword = this.SqlAdminPassword,
        PublishingServiceConnectionStrings = this.PublishingServiceConnectionStrings
      };
    }

    #endregion
  }
}