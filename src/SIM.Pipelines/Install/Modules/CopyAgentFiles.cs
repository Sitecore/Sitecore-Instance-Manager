namespace SIM.Pipelines.Install.Modules
{
  using System.Linq;
  using SIM.Pipelines.Agent;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class CopyAgentFiles : InstallProcessor
  {
    #region Methods

    protected override bool IsRequireProcessing(InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Modules.Any(m => m.IsPackage);
    }

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.Instance, "Instance");
      AgentHelper.CopyAgentFiles(args.Instance);
    }

    #endregion
  }
}