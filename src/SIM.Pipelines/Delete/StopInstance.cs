namespace SIM.Pipelines.Delete
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

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
        Log.Warn("Cannot stop instance {0}. {1}".FormatWith(args.InstanceName, ex.Message), this.GetType(), ex);
      }
    }

    #endregion
  }
}