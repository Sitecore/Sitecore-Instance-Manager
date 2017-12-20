namespace SIM.Pipelines.Backup
{
  using System.IO;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class BackupFiles : BackupProcessor
  {
    #region Methods

    #region Protected methods

    protected override long EvaluateStepsCount(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return 2;
    }

    protected override bool IsRequireProcessing(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return args.BackupFiles;
    }

    protected override void Process([NotNull] BackupArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var instance = args.Instance;
      var webRootPath = instance.WebRootPath;
      if (args.BackupClient)
      {
        BackupFolder(args, webRootPath, "WebRoot.zip");
      }
      else
      {
        BackupFolder(args, webRootPath, "WebRootNoClient.zip", "sitecore");
      }

      IncrementProgress();
      BackupFolder(args, instance.DataFolderPath, "Data.zip");
    }

    #endregion

    #region Private methods

    private void BackupFolder([NotNull] BackupArgs args, [NotNull] string path, [NotNull] string fileName, string ignore = null)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      Assert.ArgumentNotNull(path, nameof(path));
      Assert.ArgumentNotNull(fileName, nameof(fileName));

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