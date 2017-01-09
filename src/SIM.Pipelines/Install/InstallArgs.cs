namespace SIM.Pipelines.Install
{
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  public class InstallArgs : ProcessorArgs
  {
    #region Fields

    [NotNull]
    public readonly SqlConnectionStringBuilder ConnectionString;

    [NotNull]
    public readonly string DataFolderPath;

    [NotNull]
    public readonly string DatabasesFolderPath;

    public readonly bool ForceNetFramework4;

    [NotNull]
    public readonly string[] HostNames;

    public readonly string InstanceSqlPrefix;
    public readonly bool InstanceAttachSql;

    public readonly bool Is32Bit;

    public readonly bool IsClassic;

    public readonly bool PreHeat;

    [NotNull]
    public readonly string LicenseFilePath;

    [NotNull]
    public readonly IEnumerable<Product> Modules;

    [NotNull]
    public readonly string Name;

    [NotNull]
    public readonly Product Product;

    [NotNull]
    public readonly string RootFolderPath;

    [NotNull]
    public readonly string SqlServerIdentity;

    [NotNull]
    public readonly string WebRootPath;

    [NotNull]
    public readonly string WebServerIdentity;

    public readonly bool InstallRadControls;
    public readonly bool InstallDictionaries;

    #endregion

    #region Constructors

    public InstallArgs([NotNull] string name, [NotNull] string[] hosts, string instanceSqlPrefix, bool instanceAttachSql, [NotNull] Product product, [NotNull] string rootPath, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string sqlServerIdentity, [NotNull] string webServerIdentity, [NotNull] FileInfo license, bool forceNetFramework4, bool is32Bit, bool isClassic, bool installRadControls, bool installDictionaries, bool serverSideRedirect, bool increaseExecutionTimeout, bool preheat, [NotNull] IEnumerable<Product> modules)
      : this(name, hosts, instanceSqlPrefix, instanceAttachSql, product, Path.Combine(rootPath, "Website"), Path.Combine(rootPath, "Data"), Path.Combine(rootPath, "Databases"), connectionString, sqlServerIdentity, webServerIdentity, license, forceNetFramework4, is32Bit, isClassic, installRadControls, installDictionaries, serverSideRedirect, increaseExecutionTimeout, preheat, rootPath, modules)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(hosts, nameof(hosts));
      Assert.ArgumentNotNull(product, nameof(product));
      Assert.ArgumentNotNull(rootPath, nameof(rootPath));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(sqlServerIdentity, nameof(sqlServerIdentity));
      Assert.ArgumentNotNull(webServerIdentity, nameof(webServerIdentity));
      Assert.ArgumentNotNull(license, nameof(license));
      Assert.ArgumentNotNull(modules, nameof(modules));

      this.Modules = modules;
    }

    public InstallArgs([NotNull] string name, [NotNull] string[] hosts, string instanceSqlPrefix, bool instanceAttachSql, [NotNull] Product product, [NotNull] string webRootPath, [NotNull] string dataFolderPath, [NotNull] string databasesFolderPath, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string sqlServerIdentity, [NotNull] string webServerIdentity, [NotNull] FileInfo license, bool forceNetFramework4, bool is32Bit, bool isClassic, bool installRadControls, bool installDictionaries, bool serverSideRedirect, bool increaseExecutionTimeout, bool preheat, [NotNull] string rootPath, [NotNull] IEnumerable<Product> modules)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(hosts, nameof(hosts));
      Assert.ArgumentNotNull(product, nameof(product));
      Assert.ArgumentNotNull(webRootPath, nameof(webRootPath));
      Assert.ArgumentNotNull(dataFolderPath, nameof(dataFolderPath));
      Assert.ArgumentNotNull(databasesFolderPath, nameof(databasesFolderPath));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(sqlServerIdentity, nameof(sqlServerIdentity));
      Assert.ArgumentNotNull(webServerIdentity, nameof(webServerIdentity));
      Assert.ArgumentNotNull(license, nameof(license));
      Assert.ArgumentNotNull(rootPath, nameof(rootPath));
      Assert.ArgumentNotNull(modules, nameof(modules));

      this.Name = name;
      this.Modules = modules;
      this.HostNames = hosts;
      this.InstanceSqlPrefix = instanceSqlPrefix;
      InstanceAttachSql = instanceAttachSql;
      this.Product = product;
      this.ConnectionString = connectionString;
      this.DataFolderPath = dataFolderPath;
      this.DatabasesFolderPath = databasesFolderPath;
      this.WebRootPath = webRootPath;
      this.LicenseFilePath = license.FullName;
      this.SqlServerIdentity = sqlServerIdentity;
      this.WebServerIdentity = webServerIdentity;
      this.ForceNetFramework4 = forceNetFramework4;
      this.Is32Bit = is32Bit;
      this.IsClassic = isClassic;
      this.PreHeat = preheat;
      this.RootFolderPath = rootPath;
      this.InstallRadControls = installRadControls;
      this.InstallDictionaries = installDictionaries;
      this.ServerSideRedirect = serverSideRedirect;
      this.IncreaseExecutionTimeout = increaseExecutionTimeout;
    }

    #endregion

    #region Properties

    [CanBeNull]
    public Instance Instance { get; set; }

    [NotNull]
    public string InstanceName
    {
      get
      {
        return this.Name;
      }
    }

    [NotNull]
    public string PackagePath
    {
      get
      {
        return this.Product.PackagePath;
      }
    }

    public bool ServerSideRedirect { get; set; }

    public bool IncreaseExecutionTimeout { get; set; }

    #endregion

    // for install pipeline
  }
}