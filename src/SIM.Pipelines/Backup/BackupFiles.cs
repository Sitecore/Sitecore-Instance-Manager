namespace SIM.Pipelines.Backup
{
  using System.IO;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class BackupFiles : BackupProcessor
  {
    #region Methods

    #region Protected methods

    protected override long EvaluateStepsCount(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return 2;
    }

    protected override bool IsRequireProcessing(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.BackupFiles;
    }

    protected override void Process([NotNull] BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var instance = args.Instance;
      var webRootPath = instance.WebRootPath;
      if (args.BackupClient)
      {
        this.BackupFolder(args, webRootPath, "WebRoot.zip");
      }
      else
      {
        this.BackupFolder(args, webRootPath, "WebRootNoClient.zip", "sitecore");
      }

      this.IncrementProgress();
      this.BackupFolder(args, instance.DataFolderPath, "Data.zip");
    }

    #endregion

    #region Private methods

    private void BackupFolder([NotNull] BackupArgs args, [NotNull] string path, [NotNull] string fileName, string ignore = null)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(path, "path");
      Assert.ArgumentNotNull(fileName, "fileName");

      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        var backupfile = Path.Combine(args.Folder, fileName);
        FileSystem.FileSystem.Local.Zip.CreateZip(path, backupfile, ignore);
      }
    }

    #endregion

    #endregion
  }
}