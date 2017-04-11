namespace SIM.Pipelines.Restore
{
  using Sitecore.Diagnostics.Base;

  public class DeleteFiles : RestoreProcessor
  {
    #region Protected methods

    protected override long EvaluateStepsCount(RestoreArgs args)
    {
      return args.Backup.BackupDataFiles ? 2 : 1;
    }

    protected override bool IsRequireProcessing(RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return args.Backup.BackupWebsiteFiles;
    }

    protected override void Process(RestoreArgs args)
    {
      var webRootPath = args._WebRootPath;
      if (args.Backup.BackupDataFiles)
      {
        FileSystem.FileSystem.Local.Directory.DeleteIfExists(args._DataFolder);
        this.Controller.IncrementProgress();
      }

      if (args.Backup.BackupWebsiteFiles)
      {
        FileSystem.FileSystem.Local.Directory.DeleteIfExists(webRootPath, "sitecore");
      }
    }

    #endregion
  }
}