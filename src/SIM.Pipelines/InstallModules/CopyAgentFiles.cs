namespace SIM.Pipelines.InstallModules
{
  using System.Linq;
  using SIM.Pipelines.Agent;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class CopyAgentFiles : InstallModulesProcessor
  {
    #region Methods

    protected override bool IsRequireProcessing(InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return args._Modules.Any(m => m.IsPackage);
    }

    protected override void Process([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      AgentHelper.CopyAgentFiles(args.Instance);
    }

    #endregion
  }
}