#region Usings

using System.Collections.Generic;
using System.Linq;
using SIM.Base;
using SIM.Pipelines.Agent;
using SIM.Products;

#endregion

namespace SIM.Pipelines.Install.Modules
{
  #region

  

  #endregion

  /// <summary>
  ///   The perform post step actions.
  /// </summary>
  [UsedImplicitly]
  public class PerformPostStepActions : InstallProcessor
  {
    #region Fields

    /// <summary>
    ///   The done.
    /// </summary>
    private readonly List<Product> done = new List<Product>();

    #endregion

    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.Instance, "Instance");

      AgentHelper.ResetStatus(args.Instance);

      foreach (Product module in args.Modules.Where(m => m.IsPackage))
      {
        if (this.done.Contains(module))
        {
          continue;
        }

        AgentHelper.PerformPostStepAction(args.Instance, module);

        this.done.Add(module);
      }
    }

    #endregion
  }
}