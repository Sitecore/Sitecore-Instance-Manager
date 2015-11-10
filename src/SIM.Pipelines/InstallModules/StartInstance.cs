namespace SIM.Pipelines.InstallModules
{
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

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