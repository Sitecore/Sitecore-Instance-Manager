namespace SIM.Tool.Windows.UserControls.Backup
{
  using System.Collections.Generic;
  using SIM.Instances;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;

  #region

  #endregion

  public partial class ChooseBackup : IWizardStep
  {
    #region Fields

    private readonly List<InstanceBackup> _CheckBoxItems = new List<InstanceBackup>();

    #endregion

    #region Constructors

    public ChooseBackup()
    {
      this.InitializeComponent();
    }

    #endregion

    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (RestoreWizardArgs)wizardArgs;
      this._CheckBoxItems.Clear();

      this._CheckBoxItems.AddRange(args.Instance.Backups);


      this.backups.DataContext = this._CheckBoxItems;
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (RestoreWizardArgs)wizardArgs;
      args.Backup = this.backups.SelectedItem as InstanceBackup;

      return true;
    }

    #endregion
  }
}