#region Usings

using SIM.Base;
using SIM.Pipelines.Agent;

#endregion

namespace SIM.Pipelines.Install.Modules
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete agent pages.
  /// </summary>
  [UsedImplicitly]
  public class DeleteAgentPages : InstallProcessor
  {
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
      AgentHelper.DeleteAgentFiles(args.Instance);
    }

    #endregion
  }
}