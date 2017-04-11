namespace SIM.Pipelines.Install
{
  using SIM.Adapters.WebServer;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class UpdateHosts : InstallProcessor
  {
    #region Methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      foreach (var hostName in args._HostNames)
      {
        Hosts.Append(hostName);
      }
    }

    #endregion
  }
}