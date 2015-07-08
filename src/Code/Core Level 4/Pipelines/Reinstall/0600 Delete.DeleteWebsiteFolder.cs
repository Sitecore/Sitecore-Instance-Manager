#region Usings

using SIM.Base;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete website folder.
  /// </summary>
  [UsedImplicitly]
  public class DeleteWebsiteFolder : ReinstallProcessor
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

      FileSystem.Local.Directory.DeleteIfExists(args.WebRootPath);
    }

    #endregion
  }
}