#region Usings

using SIM.Adapters.WebServer;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete website.
  /// </summary>
  [UsedImplicitly]
  public class DeleteWebsite : ReinstallProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      WebServerManager.DeleteWebsite(args.WebsiteID);
    }

    #endregion
  }
}