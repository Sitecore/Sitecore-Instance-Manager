#region Usings

using SIM.Adapters.WebServer;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Install
{
  #region

  

  #endregion

  /// <summary>
  ///   The update hosts.
  /// </summary>
  [UsedImplicitly]
  public class UpdateHosts : InstallProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Hosts.Append(args.HostName);
    }

    #endregion
  }
}