namespace SIM.Tool.Windows.UserControls.Backup
{
  using SIM.Core.Common;
  using SIM.FileSystem;
  using SIM.Instances;
  using SIM.IO.Real;
  using SIM.Pipelines.Backup;
  using SIM.Pipelines.ClearBackups;
  using SIM.Pipelines.Processors;
  using SIM.Pipelines.Restore;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using Profile = Core.Common.Profile;

  public class RemoveBackupsWizardArgs : WizardArgs
  {
    #region Fields

    public Instance Instance { get; }
    private string instanceName { get; }

    #endregion

    #region Constructors

    public RemoveBackupsWizardArgs(Instance instance)
    {
      Instance = instance;
      instanceName = instance.Name;
    }

    #endregion

    #region Public properties

    public IEnumerable<string> Backups { get; set; }

    public string InstanceName
    {
      get
      {
        return instanceName;
      }
    }

    #endregion

    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      Assert.IsNotNull(Backups, "Any backup wasn\'t chosen");
      return new ClearBackupsArgs(Instance, Backups);
    }

    #endregion
  }
}