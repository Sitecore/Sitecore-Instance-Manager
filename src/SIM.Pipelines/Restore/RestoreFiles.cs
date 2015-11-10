namespace SIM.Pipelines.Restore
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class RestoreFiles : RestoreProcessor
  {
    #region Constants

    private const int A = 40;

    private const int B = 5;

    #endregion

    #region Methods

    protected override long EvaluateStepsCount(RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Backup.BackupDataFiles ? A + B : A;
    }

    protected override bool IsRequireProcessing(RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Backup.BackupWebsiteFiles;
    }

    protected override void Process([NotNull] RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var webRootPath = args.WebRootPath;
      if (args.Backup.BackupWebsiteFiles)
      {
        FileSystem.FileSystem.Local.Zip.UnpackZip(
          args.Backup.BackupWebsiteFilesNoClient ? args.Backup.WebRootNoClientFilePath : args.Backup.WebRootFilePath, 
          webRootPath, null, A);
      }

      if (args.Backup.BackupDataFiles)
      {
        this.IncrementProgress();
        FileSystem.FileSystem.Local.Zip.UnpackZip(args.Backup.DataFilePath, args.DataFolder, null, B);
      }
    }

    #endregion
  }
}