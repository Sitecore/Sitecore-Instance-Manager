#region Usings

using System.Collections.Generic;
using System.Data.SqlClient;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Processors;
using SIM.Products;

#endregion

namespace SIM.Pipelines.InstallModules
{
  #region

  

  #endregion

  /// <summary>
  ///   The install modules args.
  /// </summary>
  public class InstallModulesArgs : ProcessorArgs
  {
    #region Fields

    public readonly SqlConnectionStringBuilder ConnectionString;
    public readonly Instance Instance;

    /// <summary>
    ///   The modules.
    /// </summary>
    public readonly IEnumerable<Product> Modules;

    private readonly string instanceName;

    #endregion

    #region Constructors

    public InstallModulesArgs([NotNull] Instance instance, [NotNull] IEnumerable<Product> modules, [CanBeNull] SqlConnectionStringBuilder  connectionString = null) 
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(modules, "modules");

      this.Modules = modules;
      this.ConnectionString = connectionString;
      this.Instance = instance;
      this.instanceName = instance.Name;
    }

    public string InstanceName
    {
      get { return this.instanceName; }
    }

    #endregion
  }
}