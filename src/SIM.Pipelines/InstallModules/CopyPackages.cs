namespace SIM.Pipelines.InstallModules
{
  using System.Linq;
  using SIM.Pipelines.Agent;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class CopyPackages : InstallModulesProcessor
  {
    #region Methods

    protected override bool IsRequireProcessing(InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Modules.Any(m => m.IsPackage);
    }

    protected override void Process([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      AgentHelper.CopyPackages(args.Instance, args.Modules);
    }

    #endregion
  }
}