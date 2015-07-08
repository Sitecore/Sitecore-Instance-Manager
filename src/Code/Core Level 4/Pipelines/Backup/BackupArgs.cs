#region Usings

using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Backup
{
	public class BackupArgs : ProcessorArgs
	{
		#region Fields

		public readonly bool BackupClient;

		/// <summary>
		///   The backup databases.
		/// </summary>
		public readonly bool BackupDatabases;

    public readonly bool BackupMongoDatabases;

		/// <summary>
		///   The backup files.
		/// </summary>
		public readonly bool BackupFiles;

		public readonly string BackupName;

		/// <summary>
		///   The folder.
		/// </summary>
    [NotNull]
		public readonly string Folder;

		public readonly Instance Instance;
		private readonly string _instanceName;
		public string WebRootPath;

		public string InstanceName
		{
			get { return _instanceName; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BackupArgs"/> class.
		/// </summary>
		/// <param name="instance">
		/// The instance. 
		/// </param>
		/// <param name="backupName">
		/// Backup file path
		/// </param>
		/// <param name="backupFiles">
		/// The backup files. 
		/// </param>
		/// <param name="backupDatabases">
		/// The backup databases. 
		/// </param>
    /// <param name="backupClient">backup client files</param>
    /// <param name="backupMongoDatabases">backup mongo databases</param>
    public BackupArgs([NotNull] Instance instance, string backupName = null, bool backupFiles = false, bool backupDatabases = false, bool backupClient = false, bool backupMongoDatabases = false)
		{
			Assert.ArgumentNotNull(instance, "instance");
			BackupFiles = backupFiles;
			BackupClient = backupClient;
      BackupMongoDatabases = backupMongoDatabases;
			BackupDatabases = backupDatabases;
			Instance = instance;
			WebRootPath = instance.WebRootPath;
			BackupName = backupName;
			Folder = BackupName != null
				         ? FileSystem.Local.Directory.Ensure(instance.GetBackupFolder(BackupName))
				         : string.Empty;
			_instanceName = Instance.Name;
		}

		#endregion
	}
}