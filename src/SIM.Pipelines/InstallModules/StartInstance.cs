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
      Assert.ArgumentNotNull(args, nameof(args));

      if (this.ProcessorDefinition.Param == "nowait")
      {
        try
        {
          InstanceHelper.StartInstance(args.Instance, 500);
        }
        catch
        {
          // ignore error
        }
      }
      else
      {
        InstanceHelper.StartInstance(args.Instance);
      }
    }

    #endregion
  }
}