namespace SIM.Pipelines.Delete
{
  using System;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  [UsedImplicitly]
  public class StopInstance : DeleteProcessor
  {
    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      try
      {
        args.InstanceStop();
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "Cannot stop instance {0}. {1}", args.InstanceName, ex.Message);
      }
    }

    #endregion
  }
}