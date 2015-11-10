namespace SIM.Pipelines.InstallModules
{
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public class InstallModulesArgs : ProcessorArgs
  {
    #region Fields

    public readonly SqlConnectionStringBuilder ConnectionString;
    public readonly Instance Instance;

    public readonly IEnumerable<Product> Modules;

    private readonly string instanceName;

    #endregion

    #region Constructors

    public InstallModulesArgs([NotNull] Instance instance, [NotNull] IEnumerable<Product> modules, [CanBeNull] SqlConnectionStringBuilder connectionString = null)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(modules, "modules");

      this.Modules = modules;
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