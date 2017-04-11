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
    public readonly IEnumerable<BindingInfo> Bindings;

    [NotNull]
    public SqlConnectionStringBuilder ConnectionString { get; }

    [NotNull]
    public string DataFolderPath { get; }

    [NotNull]
    public string DatabasesFolderPath { get; }

    public bool ForceNetFramework4 { get; }

    [CanBeNull]
    public readonly ICollection<Database> InstanceDatabases;

    public bool Is32Bit { get; }

    public bool IsClassic { get; }

    [NotNull]
    public string LicenseFilePath { get; }

    [NotNull]
    public readonly IEnumerable<Product> Modules;

    [NotNull]
    public string Name { get; }

    [NotNull]
    public Product Product { get; }

    public string RootPath { get; }

    [NotNull]
    public readonly Action<bool?> StopInstance;

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
      this.ConnectionString = connectionString;
      this.Name = instance.Name;
      this.Bindings = instance.Bindings;
      this.Product = instance.Product;
      this.WebRootPath = instance.WebRootPath;
      this.RootPath = instance.RootPath;
      this.DataFolderPath = instance.DataFolderPath;
      this.DatabasesFolderPath = Path.Combine(this.RootPath, "Databases");
      this.WebServerIdentity = webServerIdentity;
      this.LicenseFilePath = license;
      this.Modules = new Product[0];
      this.IsClassic = instance.IsClassic;
      this.Is32Bit = instance.Is32Bit;
      this.ForceNetFramework4 = instance.IsNetFramework4;
      this.ServerSideRedirect = serverSideRedirect;
      this.TempFolder = Path.Combine(this.RootPath, "Temp");
      this.InstanceDatabases = instance.AttachedDatabases;
      this.instanceName = instance.Name;
      this.StopInstance = instance.Stop;
      this.WebsiteID = instance.ID;
      this.SqlPrefix = AttachDatabasesHelper.GetSqlPrefix(instance);

      var executionTimeout = UpdateWebConfigHelper.GetHttpRuntime(instance.GetWebResultConfig()).GetAttribute("executionTimeout");
      this.IncreaseExecutionTimeout = string.IsNullOrEmpty(executionTimeout) || executionTimeout != "600";
    }

    #region Properties

    #endregion

    [NotNull]
    public string PackagePath
    {
      get
      {
        return this.Product.PackagePath;
      }
    }

    #endregion

    #region Public properties

    [NotNull]
    public string InstanceName
    {
      get
      {
        return this.instanceName;
      }
    }

    #endregion
  }
}