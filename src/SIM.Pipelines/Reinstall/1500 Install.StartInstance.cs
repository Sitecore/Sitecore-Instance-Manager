namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics.Base;
  using SIM.Instances;

  #region

  #endregion

  public class StartInstance : ReinstallProcessor
  {
    #region Protected methods

    protected override void Process(ReinstallArgs args)
    {
      InstanceManager.Initialize();

      var instance = InstanceManager.GetInstance(args.InstanceName);
      Assert.IsNotNull(instance, nameof(instance));

      if (this.ProcessorDefinition.Param == "nowait")
      {
        try
        {
          InstanceHelper.StartInstance(instance, 500);
        }
        catch
        {
          // ignore error
        }
      }
      else
      {
        InstanceHelper.StartInstance(instance);
      }
    }

    #endregion
  }
}