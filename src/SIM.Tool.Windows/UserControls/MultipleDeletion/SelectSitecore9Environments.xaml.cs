using System.Collections.Generic;
using System.Windows;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.UserControls.MultipleDeletion
{
  public partial class SelectSitecore9Environments : IWizardStep, IFlowControl
  {
    private readonly List<string> _SelectedEnvironments = new List<string>();

    public SelectSitecore9Environments()
    {
      InitializeComponent();
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      if (_SelectedEnvironments.Count != 0)
      {
        return true;
      }

      MessageBox.Show("You haven't selected any of the Sitecore environments.", "Multiple deletion Sitecore 9 and later", MessageBoxButton.OK, MessageBoxImage.Warning);
      return false;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      var args = (MultipleDeletion9WizardArgs)wizardArgs;

      if (_SelectedEnvironments.Count != 0)
      {
        args._SelectedEnvironments = _SelectedEnvironments;
        args._ScriptsOnly = this.ScriptsOnly.IsChecked ?? false;
      }

      return true;
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Environments.DataContext = ((MultipleDeletion9WizardArgs)wizardArgs)._Instances;
    }

    private void OnChecked(object sender, RoutedEventArgs e)
    {
      var environmentName = ((System.Windows.Controls.CheckBox)sender).Content.ToString();
      if (!string.IsNullOrEmpty(environmentName))
      {
        _SelectedEnvironments.Add(environmentName);
      }
    }

    private void OnUnchecked(object sender, RoutedEventArgs e)
    {
      var environmentName = ((System.Windows.Controls.CheckBox)sender).Content.ToString();
      if (!string.IsNullOrEmpty(environmentName))
      {
        _SelectedEnvironments.Remove(environmentName);
      }
    }
  }
}