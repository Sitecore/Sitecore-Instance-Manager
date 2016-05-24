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
  using Sitecore.Diagnostics.Base.Annotations;

  #endregion

  public class ReinstallArgs : ProcessorArgs
  {
    #region Fields

    [NotNull]
    public readonly IEnumerable<BindingInfo> Bindings;

    [NotNull]
    public readonly SqlConnectionStringBuilder ConnectionString;

    [NotNull]
    public readonly string DataFolderPath;

    [NotNull]
    public readonly string DatabasesFolderPath;

    public readonly bool ForceNetFramework4;

    [CanBeNull]
    public readonly ICollection<Database> InstanceDatabases;

    public readonly bool Is32Bit;

    public readonly bool IsClassic;

    [NotNull]
    public readonly string LicenseFilePath;

    [NotNull]
    public readonly IEnumerable<Product> Modules;

    [NotNull]
    public readonly string Name;

    [NotNull]
    public readonly Product Product;

    public readonly string RootPath;

    [NotNull]
    public readonly Action<bool?> StopInstance;

    public readonly string TempFolder;

    [NotNull]
    public readonly string WebRootPath;

    [NotNull]
    public readonly string WebServerIdentity;

    public readonly long WebsiteID;

    [NotNull]
    public readonly string instanceName;

    public readonly bool ServerSideRedirect;

    public readonly bool IncreaseExecutionTimeout;

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