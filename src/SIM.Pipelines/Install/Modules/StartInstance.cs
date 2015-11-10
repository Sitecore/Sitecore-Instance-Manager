namespace SIM.Pipelines.Install.Modules
{
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class StartInstance : InstallProcessor
  {
    #region Methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Instance instance = args.Instance;
      Assert.IsNotNull(instance, "Instance");
      InstanceHelper.StartInstance(instance);
    }

    #endregion
  }
}