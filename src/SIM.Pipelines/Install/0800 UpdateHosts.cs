namespace SIM.Pipelines.Install
{
  using SIM.Adapters.WebServer;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class UpdateHosts : InstallProcessor
  {
    #region Methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Hosts.Append(args.HostName);
    }

    #endregion
  }
}