#region Usings

using System.IO;
using SIM.Adapters;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Install
{
  #region

  

  #endregion

  /// <summary>
  ///   The move data.
  /// </summary>
  [UsedImplicitly]
  public class MoveData : InstallProcessor
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

      DirectoryInfo uniqueFolder = new DirectoryInfo(args.UniqueTempFolder);
      DirectoryInfo[] subdirs = uniqueFolder.GetDirectories();
      int length = subdirs.Length;
      Assert.IsTrue(length == 1, "Something wrong with extracted files - in the root of the " + args.PackagePath + " package there are {0} folders instead of expected 1".FormatWith(length));
      DirectoryInfo extracted = subdirs[0];
      FileSystem.Local.Directory.MoveChild(extracted, "website", args.WebRootPath);
      FileSystem.Local.Directory.MoveChild(extracted, "data", args.DataFolderPath);
      FileSystem.Local.Directory.MoveChild(extracted, "databases", args.DatabasesFolderPath);        
    }

    #endregion
  }
}