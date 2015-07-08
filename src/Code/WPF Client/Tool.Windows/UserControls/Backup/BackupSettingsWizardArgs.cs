using System;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Backup;
using SIM.Pipelines.Processors;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.UserControls.Backup
{
  public class BackupSettingsWizardArgs : WizardArgs
  {
    public readonly Instance Instance;
    private readonly string _instanceName;

    public BackupSettingsWizardArgs(Instance instance)
    {
      Instance = instance;
      _instanceName = instance.Name;
      BackupName = string.Format("{0:yyyy-MM-dd} at {0:hh-mm-ss}", DateTime.Now);
    }

    public string BackupName { get; set; }

    public bool Databases;
    public bool MongoDatabases;
    public bool Files;
    public bool ExcludeClient;

    public string InstanceName
    {
      get { return _instanceName; }
    }

    public override ProcessorArgs ToProcessorArgs()
    {
      var backupArgs = new BackupArgs(Instance, FileSystem.Local.Path.EscapePath(BackupName, "."), Files, Databases, ExcludeClient, MongoDatabases);

      return backupArgs;
    }
  }
}
