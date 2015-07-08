#region Usings

using SIM.Base;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete and create root folder.
  /// </summary>
  [UsedImplicitly]
  public class DeleteRootFolder : ReinstallProcessor
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

      string path = args.RootPath;
      if (!string.IsNullOrEmpty(path))
      {
        FileSystem.Local.Directory.DeleteIfExists(path);

        FileSystem.Local.Directory.CreateDirectory(path);
      }
    }

    #endregion
  }
}