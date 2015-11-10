namespace SIM.Pipelines.Restore
{
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public class RestoreArgs : ProcessorArgs
  {
    #region Fields

    public readonly InstanceBackup Backup;

    public readonly Instance Instance;
    public string DataFolder;
    public string WebRootPath;
    private readonly string instanceName;

    #endregion

    #region Constructors

    public RestoreArgs([NotNull] Instance instance, InstanceBackup backup = null)
    {
      Assert.ArgumentNotNull(instance, "instance");

      // if(backup == null)
      // {
      // IEnumerable<InstanceBackup> bs = instance.GetBackups();
      // Assert.IsNotNull(bs, "There isn't any available backup", false);
      // backup = bs.OrderBy(b => b.Date).FirstOrDefault();
      // Assert.IsNotNull(backup, "There isn't any available backup", false);
      // }
      this.Backup = backup;
      this.Instance = instance;
      this.WebRootPath = instance.WebRootPath;
      this.DataFolder = instance.DataFolderPath;
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