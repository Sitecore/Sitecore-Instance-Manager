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

    public Instance Instance { get; }
    public bool _Databases;
    public bool _ExcludeClient;
    public bool _Files;
    public bool _MongoDatabases;
    private string _instanceName { get; }

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
      var backupArgs = new BackupArgs(this.Instance, FileSystem.FileSystem.Local.Path.EscapePath(this.BackupName.Trim(), "."), this._Files, this._Databases, this._ExcludeClient, this._MongoDatabases);

      return backupArgs;
    }

    #endregion
  }
}
