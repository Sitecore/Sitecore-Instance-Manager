namespace SIM.Adapters.SqlServer
{
  using System;
  using System.Data.SqlClient;
  using Sitecore.Diagnostics.Base.Annotations;
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
        return this.Name + SqlServerManager.BackupExtension;
      }
    }

    public SqlConnectionStringBuilder ConnectionString { get; set; }

    [CanBeNull]
    public virtual string FileName
    {
      get
      {
        var databaseName = this.RealName;
        var connectionString = this.ConnectionString;
        try
        {
          return SqlServerManager.Instance.GetDatabaseFileName(databaseName, connectionString);
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "An error while retrieving database file name, database: {0}, connection string: {1}", databaseName, connectionString);

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
      SqlServerManager.Instance.DeleteDatabase(this.RealName, this.ConnectionString);
    }

    public void Restore([CanBeNull] string backupFile)
    {
      SqlServerManager.Instance.RestoreDatabase(this.RealName, this.ConnectionString, backupFile);
    }

    #endregion

    #region Methods

    [NotNull]
    public SqlConnection OpenConnection()
    {
      return SqlServerManager.Instance.OpenConnection(this.ConnectionString);
    }

    #endregion

    #region Public methods

    public void Detach()
    {
      SqlServerManager.Instance.DetachDatabase(this.RealName, this.ConnectionString);
    }

    #endregion
  }
}