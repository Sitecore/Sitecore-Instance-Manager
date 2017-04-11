namespace SIM.Pipelines.Reinstall
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using SIM.Products;
  using JetBrains.Annotations;

  #endregion

  public class ReinstallArgs : ProcessorArgs
  {
    #region Fields

    [NotNull]
    public readonly IEnumerable<BindingInfo> _Bindings;

    [NotNull]
    public SqlConnectionStringBuilder ConnectionString { get; }

    [NotNull]
    public string DataFolderPath { get; }

    [NotNull]
    public string DatabasesFolderPath { get; }

    public bool ForceNetFramework4 { get; }

    [CanBeNull]
    public readonly ICollection<Database> _InstanceDatabases;

    public bool Is32Bit { get; }

    public bool IsClassic { get; }

    [NotNull]
    public string LicenseFilePath { get; }

    [NotNull]
    public readonly IEnumerable<Product> _Modules;

    [NotNull]
    public string Name { get; }

    [NotNull]
    public Product Product { get; }

    public string RootPath { get; }

    [NotNull]
    public readonly Action<bool?> _StopInstance;

    public string TempFolder { get; }

    [NotNull]
    public string WebRootPath { get; }

    [NotNull]
    public string WebServerIdentity { get; }

    public long WebsiteID { get; }

    [NotNull]
    public string instanceName { get; }

    public bool ServerSideRedirect { get; }

    public bool IncreaseExecutionTimeout { get; }
    public string SqlPrefix { get; }

    #endregion

    #region Constructors

    public ReinstallArgs(Instance instance, SqlConnectionStringBuilder connectionString, string license, string webServerIdentity, bool serverSideRedirect)
    {
      ConnectionString = connectionString;
      Name = instance.Name;
      _Bindings = instance.Bindings;
      Product = instance.Product;
      WebRootPath = instance.WebRootPath;
      RootPath = instance.RootPath;
      DataFolderPath = instance.DataFolderPath;
      DatabasesFolderPath = Path.Combine(RootPath, "Databases");
      WebServerIdentity = webServerIdentity;
      LicenseFilePath = license;
      _Modules = new Product[0];
      IsClassic = instance.IsClassic;
      Is32Bit = instance.Is32Bit;
      ForceNetFramework4 = instance.IsNetFramework4;
      ServerSideRedirect = serverSideRedirect;
      TempFolder = Path.Combine(RootPath, "Temp");
      _InstanceDatabases = instance.AttachedDatabases;
      instanceName = instance.Name;
      _StopInstance = instance.Stop;
      WebsiteID = instance.ID;
      SqlPrefix = AttachDatabasesHelper.GetSqlPrefix(instance);

      var executionTimeout = UpdateWebConfigHelper.GetHttpRuntime(instance.GetWebResultConfig()).GetAttribute("executionTimeout");
      IncreaseExecutionTimeout = string.IsNullOrEmpty(executionTimeout) || executionTimeout != "600";
    }

    #region Properties

    #endregion

    [NotNull]
    public string PackagePath
    {
      get
      {
        return Product.PackagePath;
      }
    }

    #endregion

    #region Public properties

    [NotNull]
    public string InstanceName
    {
      get
      {
        return instanceName;
      }
    }

    #endregion
  }
}