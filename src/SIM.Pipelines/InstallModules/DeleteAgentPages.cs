namespace SIM.Pipelines.InstallModules
{
  using SIM.Pipelines.Agent;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteAgentPages : InstallModulesProcessor
  {
    #region Methods

    protected override void Process([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      AgentHelper.DeleteAgentFiles(args.Instance);
    }

    #endregion
  }
}