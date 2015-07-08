#region Usings

using System.IO;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The move data.
  /// </summary>
  [UsedImplicitly]
  public class MoveData : ReinstallProcessor
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

      DirectoryInfo uniqueFolder = new DirectoryInfo(args.TempFolder);
      DirectoryInfo[] subdirs = uniqueFolder.GetDirectories();
      Assert.IsTrue(subdirs.Length == 1, "Something wrong with extracted files");
      DirectoryInfo extracted = subdirs[0];
      FileSystem.Local.Directory.MoveChild(extracted, "website", args.WebRootPath);
      FileSystem.Local.Directory.MoveChild(extracted, "data", args.DataFolderPath);
      FileSystem.Local.Directory.MoveChild(extracted, "databases", args.DatabasesFolderPath);
    }

    #endregion
  }
}