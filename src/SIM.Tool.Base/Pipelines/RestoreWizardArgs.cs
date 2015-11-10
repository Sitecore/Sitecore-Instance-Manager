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

    public readonly Instance Instance;
    private readonly string instanceName;

    #endregion

    #region Constructors

    public RestoreWizardArgs(Instance instance)
    {
      this.Instance = instance;
      this.instanceName = instance.Name;
    }

    #endregion

    #region Public properties

    public InstanceBackup Backup { get; set; }

    public string InstanceName
    {
      get
      {
        return this.instanceName;
      }
    }

    #endregion

    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      Assert.IsNotNull(this.Backup, "Any backup wasn't chosen", false);
      return new RestoreArgs(this.Instance, this.Backup);
    }

    #endregion
  }
}