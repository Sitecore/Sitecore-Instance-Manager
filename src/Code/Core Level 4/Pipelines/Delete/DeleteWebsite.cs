#region Usings

using SIM.Adapters.WebServer;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Delete
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete website.
  /// </summary>
  [UsedImplicitly]
  public class DeleteWebsite : DeleteProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      WebServerManager.DeleteWebsite(args.InstanceID);
    }

    #endregion
  }
}