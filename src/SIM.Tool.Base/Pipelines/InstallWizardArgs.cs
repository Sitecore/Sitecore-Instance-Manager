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
  using JetBrains.Annotations;

  #region

  #endregion

  public class InstallWizardArgs : InstallModulesWizardArgs
  {
    #region Properties

    public InstallWizardArgs()
    {
      Init(this);
    }

    public InstallWizardArgs(Instance instance) : base(instance)
    {
      Init(this);
    }

    private static void Init(InstallWizardArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      args.SkipRadControls = LastTimeOption(nameof(args.SkipRadControls)) ?? !Settings.CoreInstallRadControls.Value;
      args.SkipDictionaries = LastTimeOption(nameof(args.SkipDictionaries)) ?? !Settings.CoreInstallDictionaries.Value;
      args.PreHeat = LastTimeOption(nameof(args.PreHeat)) ?? true;
      args.ServerSideRedirect = LastTimeOption(nameof(args.ServerSideRedirect)) ?? Settings.CoreInstallNotFoundTransfer.Value;
      args.IncreaseExecutionTimeout = LastTimeOption(nameof(args.IncreaseExecutionTimeout)) ?? !string.IsNullOrEmpty(Settings.CoreInstallHttpRuntimeExecutionTimeout.Value);
    }

    public new Instance Instance
    {
      get
      {
        return InstanceManager.Default.GetInstance(this.InstanceName);
      }
    }

    public AppPoolInfo InstanceAppPoolInfo { get; set; }

    public SqlConnectionStringBuilder InstanceConnectionString { get; set; }

    public string[] InstanceHostNames { get; set; }

    public string InstanceSqlPrefix { get; set; }

    public new string InstanceName { get; set; }

    public Product InstanceProduct { get; set; }

    public string InstanceRootPath { get; set; }

    public string InstanceWebRootPath { get; set; }

    public FileInfo LicenseFileInfo { get; set; }

    public override Product Product { get; set; }

    public bool PreHeat { get; set; }

    #endregion

    #region Public Methods

    public override ProcessorArgs ToProcessorArgs()
    {
      var skipRadControls = this.SkipRadControls;
      var skipDictionaries = this.SkipDictionaries;
      var serverSideRedirect = this.ServerSideRedirect;
      var increaseExecutionTimeout = this.IncreaseExecutionTimeout;
      var installRadControls = !((bool)skipRadControls);
      var installDictionaries = !((bool)skipDictionaries);
      var preheat = this.PreHeat;

      return new InstallArgs(this.InstanceName, this.InstanceHostNames, this.InstanceSqlPrefix, this.InstanceAttachSql, this.InstanceProduct, this.InstanceRootPath, this.InstanceConnectionString, SqlServerManager.Instance.GetSqlServerAccountName(this.InstanceConnectionString), Settings.CoreInstallWebServerIdentity.Value, this.LicenseFileInfo, this.InstanceAppPoolInfo.FrameworkVersion == "v4.0", this.InstanceAppPoolInfo.Enable32BitAppOnWin64, !this.InstanceAppPoolInfo.ManagedPipelineMode, installRadControls, installDictionaries, (bool)serverSideRedirect, (bool)increaseExecutionTimeout, (bool)preheat, this._Modules);
    }

    public static bool? LastTimeOption(string option)
    {
      var filePath = GetLastTimeOptionFilePath(option);
      if (!File.Exists(filePath))
      {
        return null;
      }

      return File.ReadAllText(filePath).StartsWith("1");
    }

    public static void SaveLastTimeOption(string option, bool value)
    {
      var filePath = GetLastTimeOptionFilePath(option);

      File.WriteAllText(filePath, value ? "1" : "0");
    }

    [NotNull]
    private static string GetLastTimeOptionFilePath(string option)
    {
      var filePath = Path.Combine(ApplicationManager.TempFolder, $"{typeof(InstallWizardArgs).FullName}.{option}.txt");
      return filePath;
    }

    #endregion

    #region Public properties

    public string InstanceRootName { get; set; }

    public bool SkipDictionaries { get; set; }

    public bool SkipRadControls { get; set; }

    public bool ServerSideRedirect { get; set; }

    public bool IncreaseExecutionTimeout { get; set; }

    public bool InstanceAttachSql { get; set; }

    #endregion
  }
}