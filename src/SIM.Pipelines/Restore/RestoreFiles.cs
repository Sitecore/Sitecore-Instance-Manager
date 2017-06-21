namespace SIM.Pipelines.Restore
{
    using Sitecore.Diagnostics.Base;
    using JetBrains.Annotations;

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
            Assert.ArgumentNotNull(args, nameof(args));

            return args.Backup.BackupDataFiles ? A + B : A;
        }

        protected override bool IsRequireProcessing(RestoreArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            return args.Backup.BackupWebsiteFiles;
        }

        protected override void Process([NotNull] RestoreArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            var webRootPath = args._WebRootPath;
            if (args.Backup.BackupWebsiteFiles)
            {
                //Remove folder content before unzipping the backup into the same location. 
                //Potentially dangerous operation. This should probably be triggered by a flag. 
                var directoryProvider = new FileSystem.DirectoryProvider(FileSystem.FileSystem.Local);
                directoryProvider.Delete(webRootPath);

                FileSystem.FileSystem.Local.Zip.UnpackZip(
          args.Backup.BackupWebsiteFilesNoClient ? args.Backup.WebRootNoClientFilePath : args.Backup.WebRootFilePath,
          webRootPath, null, A);
            }

            if (args.Backup.BackupDataFiles)
            {
                IncrementProgress();
                FileSystem.FileSystem.Local.Zip.UnpackZip(args.Backup.DataFilePath, args._DataFolder, null, B);
            }
        }

        #endregion
    }
}