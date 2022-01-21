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

    private readonly List<string> _SelectedInstances = new List<string>();

    #endregion

    #region Constructors

    public SelectInstances()
    {
      InitializeComponent();
    }

    #endregion

    #region Public methods

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      if (_SelectedInstances.Count != 0)
      {
        return true;
      }

      MessageBox.Show("You haven't selected any of the instances", "Multiple deletion Sitecore 8 and earlier", MessageBoxButton.OK, MessageBoxImage.Warning);
      return false;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      var args = (MultipleDeletionWizardArgs)wizardArgs;

      if (_SelectedInstances.Count != 0)
      {
        args._SelectedInstances = _SelectedInstances;
      }

      return true;
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      Instances.DataContext = ((MultipleDeletionWizardArgs)wizardArgs)._Instances;
    }

    private void OnChecked(object sender, RoutedEventArgs e)
    {
      var instanceName = ((System.Windows.Controls.CheckBox)sender).Content.ToString();
      if (!string.IsNullOrEmpty(instanceName))
      {
        _SelectedInstances.Add(instanceName);
      }
    }

    private void OnUnchecked(object sender, RoutedEventArgs e)
    {
      var instanceName = ((System.Windows.Controls.CheckBox)sender).Content.ToString();
      if (!string.IsNullOrEmpty(instanceName))
      {
        _SelectedInstances.Remove(instanceName);
      }
    }

    #endregion
  }
}