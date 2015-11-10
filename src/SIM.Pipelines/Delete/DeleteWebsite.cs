namespace SIM.Pipelines.Delete
{
  using SIM.Adapters.WebServer;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteWebsite : DeleteProcessor
  {
    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      WebServerManager.DeleteWebsite(args.InstanceID);
    }

    #endregion
  }
}