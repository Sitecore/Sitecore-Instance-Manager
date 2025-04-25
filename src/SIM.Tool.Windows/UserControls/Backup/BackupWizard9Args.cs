namespace SIM.Tool.Windows.UserControls.Backup
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using SIM.FileSystem;
  using SIM.Instances;
  using SIM.Pipelines.Backup;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;

  public class BackupWizard9Args : WizardArgs
  {
    #region Fields

    public Instance Instance { get; }
    private string _instanceName { get; }
    public bool _BackupDatabase;
    public bool _BackupClient;
    public bool _Files;
    private string _BackupName;

    public bool _WipeSqlServerCredentials;

    #endregion

    #region Constructors

    public BackupWizard9Args(Instance instance)
    {
      Instance = instance;
      _instanceName = instance.Name;
    }

    #endregion

    #region Public properties

    public string BackupName { get; set; }

    public string InstanceName
    {
      get
      {
        return _instanceName;
      }
    }
    public IEnumerable<string> SelectedDatabases { get; set; }

    #endregion

    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      var backupArgs = new Backup9Args(Instance, ProfileManager.GetConnectionString(),
        PathUtils.EscapePath(BackupName.Trim(), "."), _Files, _BackupClient,
        _BackupDatabase, SelectedDatabases);

      return backupArgs;
    }

    #endregion
  }
}
