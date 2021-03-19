namespace SIM.Pipelines.InstallModules
{
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  public class InstallModulesArgs : ProcessorArgs
  {
    #region Fields

    public SqlConnectionStringBuilder ConnectionString { get; }
    public Instance Instance { get; }

    public readonly IEnumerable<Product> _Modules;

    private string instanceName { get; }

    public string Cookies { get; }

    public IDictionary<string, string> Headers { get; }

    #endregion

    #region Constructors

    public InstallModulesArgs([NotNull] Instance instance, [NotNull] IEnumerable<Product> modules, [CanBeNull] SqlConnectionStringBuilder connectionString = null, [CanBeNull] string cookies = null, [CanBeNull] IDictionary<string, string> headers = null)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(modules, nameof(modules));

      _Modules = modules;
      ConnectionString = connectionString;
      Instance = instance;
      instanceName = instance.Name;
      Cookies = cookies;
      Headers = headers;
    }

    #endregion

    #region Public properties

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