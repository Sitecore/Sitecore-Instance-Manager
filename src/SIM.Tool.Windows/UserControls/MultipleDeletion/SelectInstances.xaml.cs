namespace SIM.Tool.Windows.UserControls.MultipleDeletion
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base.Wizards;

  public partial class SelectInstances : IWizardStep, IFlowControl
  {
    #region Fields

    private readonly List<string> _selectedInstances = new List<string>();

    #endregion

    #region Constructors

    public SelectInstances()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Public methods

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      if (this._selectedInstances.Count != 0)
      {
        return true;
      }

      MessageBox.Show("You haven't selected any of the instances");
      return false;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      var args = (MultipleDeletionWizardArgs)wizardArgs;

      if (this._selectedInstances.Count != 0)
      {
        args.SelectedInstances = this._selectedInstances;
      }

      return true;
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      this.Instances.DataContext = InstanceManager.Instances.OrderBy(instance => instance.Name);
    }

    private void OnChecked(object sender, RoutedEventArgs e)
    {
      var instanceName = ((System.Windows.Controls.CheckBox)sender).Content.ToString();
      if (!string.IsNullOrEmpty(instanceName))
      {
        this._selectedInstances.Add(instanceName);
      }
    }

    private void OnUnchecked(object sender, RoutedEventArgs e)
    {
      var instanceName = ((System.Windows.Controls.CheckBox)sender).Content.ToString();
      if (!string.IsNullOrEmpty(instanceName))
      {
        this._selectedInstances.Remove(instanceName);
      }
    }

    #endregion
  }
}