#region Usings

using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Agent;

#endregion

namespace SIM.Pipelines.InstallModules
{
  #region

  

  #endregion

  /// <summary>
  ///   The start instance.
  /// </summary>
  [UsedImplicitly]
  public class StartInstance : InstallModulesProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      InstanceHelper.StartInstance(args.Instance);
    }

    #endregion
  }
}