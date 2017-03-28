namespace SIM.Pipelines.Reinstall
{
  using System;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  [UsedImplicitly]
  public class StopInstance : ReinstallProcessor
  {
    #region Methods

    protected override void Process(ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      try
      {
        args.StopInstance(true);
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"Cannot stop instance {args.InstanceName}. {ex.Message}");
      }
    }

    #endregion
  }
}