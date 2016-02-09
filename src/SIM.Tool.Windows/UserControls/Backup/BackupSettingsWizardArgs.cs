namespace SIM.Tool.Windows.UserControls.Backup
{
  using System;
  using SIM.Instances;
  using SIM.Pipelines.Backup;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Wizards;

  public class BackupSettingsWizardArgs : WizardArgs
  {
    #region Fields

    public readonly Instance Instance;
    public bool Databases;
    public bool ExcludeClient;
    public bool Files;
    public bool MongoDatabases;
    private readonly string _instanceName;

    #endregion

    #region Constructors

    public BackupSettingsWizardArgs(Instance instance)
    {
      this.Instance = instance;
      this._instanceName = instance.Name;
      this.BackupName = string.Format("{0:yyyy-MM-dd} at {0:hh-mm-ss}", DateTime.Now);
    }

    #endregion

    #region Public properties

    public string BackupName { get; set; }

    public string InstanceName
    {
      get
      {
        return this._instanceName;
      }
    }

    #endregion

    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      var backupArgs = new BackupArgs(this.Instance, FileSystem.FileSystem.Local.Path.EscapePath(this.BackupName.Trim(), "."), this.Files, this.Databases, this.ExcludeClient, this.MongoDatabases);

      return backupArgs;
    }

    #endregion
  }
}
