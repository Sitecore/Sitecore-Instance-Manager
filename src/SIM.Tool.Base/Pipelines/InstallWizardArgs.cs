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
  using SIM.IO;
  using SIM.IO.Real;

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

      args.InstallRoles = LastTimeOption(nameof(args.InstallRoles)) ?? ""; 
      args.SkipRadControls = LastTimeOption(nameof(args.SkipRadControls))?.StartsWith("1") ?? !Settings.CoreInstallRadControls.Value;
      args.SkipDictionaries = LastTimeOption(nameof(args.SkipDictionaries))?.StartsWith("1") ?? !Settings.CoreInstallDictionaries.Value;
      args.PreHeat = LastTimeOption(nameof(args.PreHeat))?.StartsWith("1") ?? true;
      args.ServerSideRedirect = LastTimeOption(nameof(args.ServerSideRedirect))?.StartsWith("1") ?? Settings.CoreInstallNotFoundTransfer.Value;
      args.IncreaseExecutionTimeout = LastTimeOption(nameof(args.IncreaseExecutionTimeout))?.StartsWith("1") ?? !string.IsNullOrEmpty(Settings.CoreInstallHttpRuntimeExecutionTimeout.Value);
    }

    public new Instance Instance
    {
      get
      {
        return InstanceManager.Default.GetInstance(InstanceName);
      }
    }

    protected IFileSystem FileSystem { get; } = new RealFileSystem();

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
      var skipRadControls = SkipRadControls;
      var skipDictionaries = SkipDictionaries;
      var serverSideRedirect = ServerSideRedirect;
      var increaseExecutionTimeout = IncreaseExecutionTimeout;
      var installRadControls = !((bool)skipRadControls);
      var installDictionaries = !((bool)skipDictionaries);
      var preheat = PreHeat;
      var installRoles = InstallRoles;

      return new InstallArgs(InstanceName, InstanceHostNames, InstanceSqlPrefix, InstanceAttachSql, InstanceProduct, FileSystem.ParseFolder(InstanceRootPath), InstanceConnectionString, SqlServerManager.Instance.GetSqlServerAccountName(InstanceConnectionString), Settings.CoreInstallWebServerIdentity.Value, FileSystem.ParseFile(LicenseFileInfo), InstanceAppPoolInfo.FrameworkVersion == "v4.0", InstanceAppPoolInfo.Enable32BitAppOnWin64, !InstanceAppPoolInfo.ManagedPipelineMode, installRadControls, installDictionaries, (bool)serverSideRedirect, (bool)increaseExecutionTimeout, (bool)preheat, installRoles, _Modules);
    }

    public static string LastTimeOption(string option)
    {
      var filePath = GetLastTimeOptionFilePath(option);
      if (!File.Exists(filePath))
      {
        return null;
      }

      return File.ReadAllText(filePath);
    }

    public static void SaveLastTimeOption(string option, object value)
    {
      var filePath = GetLastTimeOptionFilePath(option);

      if (value.GetType() == typeof(bool))
      {
        File.WriteAllText(filePath, (bool)value ? "1" : "0");
      }
      else if(value.GetType() == typeof(string))
      {
        File.WriteAllText(filePath, (string)value);
      }
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

    public string InstallRoles { get; set; }

    #endregion
  }
}