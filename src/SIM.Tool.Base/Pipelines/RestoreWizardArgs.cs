namespace SIM.Tool.Base.Pipelines
{
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using SIM.Pipelines.Restore;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;

  public class RestoreWizardArgs : WizardArgs
  {
    #region Fields

    public Instance Instance { get; }
    private string instanceName { get; }

    #endregion

    #region Constructors

    public RestoreWizardArgs(Instance instance)
    {
      Instance = instance;
      instanceName = instance.Name;
    }

    #endregion

    #region Public properties

    public InstanceBackup Backup { get; set; }

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
      Assert.IsNotNull(Backup, "Any backup wasn\'t chosen");
      return new RestoreArgs(Instance, Backup);
    }

    #endregion
  }
}