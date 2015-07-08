#region Usings

using SIM.Base;
using SIM.Pipelines.Agent;

#endregion

namespace SIM.Pipelines.InstallModules
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete agent pages.
  /// </summary>
  [UsedImplicitly]
  public class DeleteAgentPages : InstallModulesProcessor
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

      AgentHelper.DeleteAgentFiles(args.Instance);
    }

    #endregion
  }
}