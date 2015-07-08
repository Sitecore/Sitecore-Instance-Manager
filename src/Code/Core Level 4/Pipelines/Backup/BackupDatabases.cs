#region Usings

using System.IO;
using System.Linq;
using SIM.Adapters.SqlServer;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Backup
{
	[UsedImplicitly]
	public class BackupDatabases : BackupProcessor
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

			return args.Instance.AttachedDatabases.Count();
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

			return args.BackupDatabases;
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
			Assert.IsNotNull(instance, "instance");
			var backupDatabasesFolder = FileSystem.Local.Directory.Ensure(Path.Combine(args.Folder, "Databases"));

			foreach (Database database in instance.AttachedDatabases)
			{
				Backup(database, backupDatabasesFolder);
				IncrementProgress();
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// The backup.
		/// </summary>
		/// <param name="database">
		/// The database. 
		/// </param>
		/// <param name="folder">
		/// The folder. 
		/// </param>
		private void Backup([NotNull] Database database, [NotNull] string folder)
		{
			Assert.ArgumentNotNull(database, "database");
			Assert.ArgumentNotNull(folder, "folder");

			var connectionString = database.ConnectionString;

			using (var connection = SqlServerManager.Instance.OpenConnection(connectionString))
			{
				var databaseName = database.RealName;
				var fileName = Path.Combine(folder, database.BackupFilename);
				Log.Info("Backing up the '{0}' database to the '{1}' file".FormatWith(databaseName, fileName), this);

				var command = "BACKUP DATABASE [" + databaseName + "] TO  DISK = N'" + fileName +
				              "' WITH NOFORMAT, NOINIT,  NAME = N'" + databaseName +
				              " initial backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
				SqlServerManager.Instance.Execute(connection, command);
			}
		}

		#endregion

		#endregion
	}
}