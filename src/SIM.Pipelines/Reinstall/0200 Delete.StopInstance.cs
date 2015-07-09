namespace SIM.Pipelines.Reinstall
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class StopInstance : ReinstallProcessor
  {
    #region Methods

    protected override void Process(ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      try
      {
        args.StopInstance(true);
      }
      catch (Exception ex)
      {
        Log.Warn("Cannot stop instance {0}. {1}".FormatWith(args.InstanceName, ex.Message), this.GetType(), ex);
      }
    }

    #endregion
  }
}