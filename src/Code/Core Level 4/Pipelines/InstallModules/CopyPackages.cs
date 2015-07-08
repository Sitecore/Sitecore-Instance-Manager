#region Usings

using System.Linq;
using SIM.Base;
using SIM.Pipelines.Agent;

#endregion

namespace SIM.Pipelines.InstallModules
{
  #region

  

  #endregion

  /// <summary>
  ///   The copy packages.
  /// </summary>
  [UsedImplicitly]
  public class CopyPackages : InstallModulesProcessor
  {
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
    protected override bool IsRequireProcessing(InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Modules.Any(m => m.IsPackage);
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      AgentHelper.CopyPackages(args.Instance, args.Modules);
    }

    #endregion
  }
}