namespace SIM.Pipelines.Restore
{
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using System.Data.SqlClient;

  #region

  #endregion

  public class RestoreArgs : ProcessorArgs
  {
    #region Fields

    public InstanceBackup Backup { get; }

    public Instance Instance { get; }
    public string _DataFolder;
    public string _WebRootPath;
    private string instanceName { get; }

    public SqlConnectionStringBuilder ManagementConnectionString { get; }

    #endregion

    #region Constructors

    public RestoreArgs([NotNull] Instance instance, SqlConnectionStringBuilder managementConnectionString, InstanceBackup backup = null)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(managementConnectionString, nameof(managementConnectionString));

      // if(backup == null)
      // {
      // IEnumerable<InstanceBackup> bs = instance.GetBackups();
      // Assert.IsNotNull(bs, "There isn't any available backup", false);
      // backup = bs.OrderBy(b => b.Date).FirstOrDefault();
      // Assert.IsNotNull(backup, "There isn't any available backup", false);
      // }
      Backup = backup;
      Instance = instance;
      _WebRootPath = instance.WebRootPath;
      _DataFolder = instance.DataFolderPath;
      instanceName = instance.Name;
      ManagementConnectionString = managementConnectionString;
    }

    #endregion

    #region Public properties

    public string InstanceName
    {
      get
      {
        return instanceName;
      }
    }

    #endregion
  }
}