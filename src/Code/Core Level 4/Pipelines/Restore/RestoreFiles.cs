#region Usings

using SIM.Adapters;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Restore
{
  #region

  

  #endregion

  /// <summary>
  ///   The restore files.
  /// </summary>
  [UsedImplicitly]
  public class RestoreFiles : RestoreProcessor
  {
    #region Constants

    /// <summary>
    ///   The a.
    /// </summary>
    private const int A = 40;

    /// <summary>
    ///   The b.
    /// </summary>
    private const int B = 5;

    #endregion

    #region Methods

    /// <summary>
    /// The evaluate steps count.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The evaluate steps count. 
    /// </returns>
    protected override long EvaluateStepsCount(RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Backup.BackupDataFiles ? A + B : A;
    }

    /// <summary>
    /// The is require processing.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The is require processing. 
    /// </returns>
    protected override bool IsRequireProcessing(RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Backup.BackupWebsiteFiles;
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var webRootPath = args.WebRootPath;
      if(args.Backup.BackupWebsiteFiles)
      {
        FileSystem.Local.Zip.UnpackZip(
          args.Backup.BackupWebsiteFilesNoClient ? args.Backup.WebRootNoClientFilePath : args.Backup.WebRootFilePath,
          webRootPath, null, A);
      }
      if (args.Backup.BackupDataFiles)
      {
        this.IncrementProgress();
        FileSystem.Local.Zip.UnpackZip(args.Backup.DataFilePath, args.DataFolder, null, B);
      }
    }

    #endregion
  }
}