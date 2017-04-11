namespace SIM.Pipelines.Restore
{
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

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

    #endregion

    #region Constructors

    public RestoreArgs([NotNull] Instance instance, InstanceBackup backup = null)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));

      // if(backup == null)
      // {
      // IEnumerable<InstanceBackup> bs = instance.GetBackups();
      // Assert.IsNotNull(bs, "There isn't any available backup", false);
      // backup = bs.OrderBy(b => b.Date).FirstOrDefault();
      // Assert.IsNotNull(backup, "There isn't any available backup", false);
      // }
      this.Backup = backup;
      this.Instance = instance;
      this._WebRootPath = instance.WebRootPath;
      this._DataFolder = instance.DataFolderPath;
      this.instanceName = instance.Name;
    }

    #endregion

    #region Public properties

    public string InstanceName
    {
      get
      {
        return this.instanceName;
      }
    }

    #endregion
  }
}