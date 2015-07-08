#region Usings

using System.Collections.Generic;
using System.Linq;
using SIM.Base;
using SIM.Instances;
using SIM.Products;

#endregion

namespace SIM.Pipelines.Install.Modules
{
  #region

  

  #endregion

  /// <summary>
  ///   The install actions.
  /// </summary>
  public class InstallActions : InstallProcessor
  {
    #region Fields

    /// <summary>
    ///   The done.
    /// </summary>
    private readonly List<Product> done = new List<Product>();

    #endregion

    #region Methods

    /// <summary>
    /// The is require processing.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The is require processing. 
    /// </returns>
    protected override bool IsRequireProcessing(InstallArgs args)
    {
      return !this.ProcessorDefinition.Param.EqualsIgnoreCase("archive") || (args.Modules != null && args.Modules.Any(m => m != null && m.IsArchive));
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process(InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.IsNotNull(args.Instance, "Instance");

      Instance instance = args.Instance;
      IEnumerable<Product> modules = args.Modules;
      string param = this.ProcessorDefinition.Param;
      ConfigurationActions.ExecuteActions(instance, modules.ToArray(), this.done, param, args.ConnectionString, this.Controller);
    }

    #endregion
  }
}