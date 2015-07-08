#region Usings

using SIM.Base;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete data folder.
  /// </summary>
  [UsedImplicitly]
  public class DeleteDataFolder : ReinstallProcessor
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

      string path = args.DataFolderPath;
      FileSystem.Local.Directory.DeleteIfExists(path);
    }

    #endregion
  }
}