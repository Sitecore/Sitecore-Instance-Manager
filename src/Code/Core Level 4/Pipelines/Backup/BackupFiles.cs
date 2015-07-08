#region Usings

using System.IO;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Backup
{

	#region

	#endregion

	[UsedImplicitly]
	public class BackupFiles : BackupProcessor
	{
		#region Methods

		#region Protected methods

		/// <summary>
		/// The evaluate steps count.
		/// </summary>
		/// <param name="args">
		/// The args. 
		/// </param>
		/// <returns>
		/// The evaluate steps count. 
		/// </returns>
		protected override long EvaluateStepsCount(BackupArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			return 2;
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
		protected override bool IsRequireProcessing(BackupArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			return args.BackupFiles;
		}

		/// <summary>
		/// The process.
		/// </summary>
		/// <param name="args">
		/// The args. 
		/// </param>
		protected override void Process([NotNull] BackupArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

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

		/// <summary>
		/// The backup folder.
		/// </summary>
		/// <param name="args">
		/// The args. 
		/// </param>
		/// <param name="path">
		/// The path. 
		/// </param>
		/// <param name="fileName">
		/// The file name. 
		/// </param>
		/// <param name="ignore"></param>
		private void BackupFolder([NotNull] BackupArgs args, [NotNull] string path, [NotNull] string fileName, string ignore = null)
		{
			Assert.ArgumentNotNull(args, "args");
			Assert.ArgumentNotNull(path, "path");
			Assert.ArgumentNotNull(fileName, "fileName");

			if (FileSystem.Local.Directory.Exists(path))
			{
				var backupfile = Path.Combine(args.Folder, fileName);
				FileSystem.Local.Zip.CreateZip(path, backupfile, ignore);
			}
		}

		#endregion

		#endregion
	}
}