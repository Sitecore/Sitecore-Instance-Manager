#region Usings

using System;
using System.Data.SqlClient;
using SIM.Base;

#endregion

namespace SIM.Adapters.SqlServer
{
  #region

  

  #endregion

  /// <summary>
  ///   The database.
  /// </summary>
  public class Database
  {
    #region Properties

    /// <summary>
    ///   Gets the backup filename.
    /// </summary>
    [NotNull]
    public string BackupFilename
    {
      get
      {
        return this.Name + SqlServerManager.BackupExtension;
      }
    }

    /// <summary>
    ///   Gets or sets ConnectionString.
    /// </summary>
    public SqlConnectionStringBuilder ConnectionString { get; set; }

    /// <summary>
    ///   Gets the name of the file.
    /// </summary>
    /// <value> The name of the file. </value>
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
          Log.Warn("An error while retrieving database file name, database: {0}, connection string: {1}".FormatWith(databaseName, connectionString), this, ex);

          return null;
        }
      }
    }

    /// <summary>
    ///   Gets or sets the name.
    /// </summary>
    /// <value> The name. </value>
    public string Name { get; set; }

    /// <summary>
    ///   Gets or sets the name of the real.
    /// </summary>
    /// <value> The name of the real. </value>
    public string RealName { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    ///   Deletes this instance.
    /// </summary>
    public void Delete()
    {
      SqlServerManager.Instance.DeleteDatabase(this.RealName, this.ConnectionString);
    }

    /// <summary>
    /// Restores the specified backup file.
    /// </summary>
    /// <param name="backupFile">
    /// The backup file. 
    /// </param>
    public void Restore([CanBeNull] string backupFile)
    {
      SqlServerManager.Instance.RestoreDatabase(this.RealName, this.ConnectionString, backupFile);
    }

    #endregion

    #region Methods

    /// <summary>
    ///   Opens the connection.
    /// </summary>
    /// <returns> The connection. </returns>
    [NotNull]
    public SqlConnection OpenConnection()
    {
      return SqlServerManager.Instance.OpenConnection(this.ConnectionString);
    }

    #endregion

    public void Detach()
    {
      SqlServerManager.Instance.DetachDatabase(this.RealName, this.ConnectionString);
    }
  }
}