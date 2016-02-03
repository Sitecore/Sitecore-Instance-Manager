namespace SIM.Tool.Base.Pipelines
{
  using System.Data.SqlClient;
  using System.IO;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using SIM.Pipelines.Install;
  using SIM.Pipelines.Processors;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;

  #region

  #endregion

  public class InstallWizardArgs : InstallModulesWizardArgs
  {
    #region Properties

    public new Instance Instance
    {
      get
      {
        return InstanceManager.GetInstance(this.InstanceName);
      }
    }

    public AppPoolInfo InstanceAppPoolInfo { get; set; }

    public SqlConnectionStringBuilder InstanceConnectionString { get; set; }

    public string InstanceHost { get; set; }

    public new string InstanceName { get; set; }

    public Product InstanceProduct { get; set; }

    public string InstanceRootPath { get; set; }

    public string InstanceWebRootPath { get; set; }

    public FileInfo LicenseFileInfo { get; set; }

    public override Product Product { get; set; }

    #endregion

    #region Public Methods

    public override ProcessorArgs ToProcessorArgs()
    {
      var skipRadControls = this.SkipRadControls;
      Assert.IsTrue(skipRadControls != null, "skipRadControls");

      var skipDictionaries = this.SkipDictionaries;
      Assert.IsTrue(skipDictionaries != null, "skipDictionaries");

      var serverSideRedirect = this.ServerSideRedirect;
      Assert.IsTrue(serverSideRedirect != null, "serverSideRedirect");

      var increaseExecutionTimeout = this.IncreaseExecutionTimeout;
      Assert.IsTrue(increaseExecutionTimeout != null, "increaseExecutionTimeout");

      var installRadControls = !((bool)skipRadControls);
      var installDictionaries = !((bool)skipDictionaries);
      return new InstallArgs(this.InstanceName, this.InstanceHost, this.InstanceProduct, this.InstanceRootPath, this.InstanceConnectionString, SqlServerManager.Instance.GetSqlServerAccountName(this.InstanceConnectionString), Settings.CoreInstallWebServerIdentity.Value, this.LicenseFileInfo, this.InstanceAppPoolInfo.FrameworkVersion == "v4.0", this.InstanceAppPoolInfo.Enable32BitAppOnWin64, !this.InstanceAppPoolInfo.ManagedPipelineMode, installRadControls, installDictionaries, (bool)serverSideRedirect, (bool)increaseExecutionTimeout, this.Modules);
    }

    #endregion

    #region Public properties

    public string InstanceRootName { get; set; }

    #endregion
  }
}