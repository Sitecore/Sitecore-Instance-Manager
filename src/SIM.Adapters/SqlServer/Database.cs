namespace SIM.Adapters.SqlServer
{
  using System;
  using System.Data.SqlClient;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public class Database
  {
    #region Properties

    [NotNull]
    public string BackupFilename
    {
      get
      {
        return Name + SqlServerManager.BackupExtension;
      }
    }

    public SqlConnectionStringBuilder ConnectionString { get; set; }

    [CanBeNull]
    public virtual string FileName
    {
      get
      {
        var databaseName = RealName;
        var connectionString = ConnectionString;
        try
        {
          return SqlServerManager.Instance.GetDatabaseFileName(databaseName, connectionString);
        }
        catch (Exception ex)
        {
          Log.Warn(ex, $"An error while retrieving database file name, database: {databaseName}, connection string: {connectionString}");
          return null;
        }
      }
    }

    public string Name { get; set; }

    public string RealName { get; set; }

    #endregion

    #region Public Methods

    public void Delete()
    {
      SqlServerManager.Instance.DeleteDatabase(RealName, ConnectionString);
    }

    public void Restore([CanBeNull] string backupFile)
    {
      SqlServerManager.Instance.RestoreDatabase(RealName, ConnectionString, backupFile);
    }

    #endregion

    #region Methods

    #endregion

    #region Public methods

    #endregion
  }
}