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

    #endregion

    #region Constructors

    public InstallModulesArgs([NotNull] Instance instance, [NotNull] IEnumerable<Product> modules, [CanBeNull] SqlConnectionStringBuilder connectionString = null)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(modules, nameof(modules));

      this._Modules = modules;
      this.ConnectionString = connectionString;
      this.Instance = instance;
      this.instanceName = instance.Name;
    }

    #endregion

    #region Public properties

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