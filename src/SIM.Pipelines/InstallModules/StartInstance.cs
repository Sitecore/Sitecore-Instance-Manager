namespace SIM.Pipelines.InstallModules
{
  using SIM.Instances;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class StartInstance : InstallModulesProcessor
  {
    #region Methods

    protected override void Process([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      InstanceHelper.StartInstance(args.Instance);
    }

    #endregion
  }
}