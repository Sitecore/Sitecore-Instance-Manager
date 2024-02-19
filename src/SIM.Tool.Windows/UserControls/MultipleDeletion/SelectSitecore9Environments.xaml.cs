using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SIM.Instances;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.UserControls.MultipleDeletion
{
  public partial class SelectSitecore9Environments : IWizardStep, IFlowControl
  {
    private List<IEnvironmentCheckBox> EnvironmentCheckBoxItems;

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
      if (EnvironmentCheckBoxItems != null && EnvironmentCheckBoxItems.Where(item => item.IsChecked).Any())
      {
        return true;
      }

      MessageBox.Show("You haven't selected any of the Sitecore environments.", "Multiple deletion Sitecore 9 and later", MessageBoxButton.OK, MessageBoxImage.Warning);
      return false;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      var args = (MultipleDeletion9WizardArgs)wizardArgs;

      if (EnvironmentCheckBoxItems != null && EnvironmentCheckBoxItems.Where(item => item.IsChecked).Any())
      {
        args._SelectedEnvironments = EnvironmentCheckBoxItems.Where(item => item.IsChecked).Select(item => item.Name).ToList();
        args._ScriptsOnly = this.ScriptsOnly.IsChecked ?? false;
      }

      return true;
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      var args = (MultipleDeletion9WizardArgs)wizardArgs;

      EnvironmentCheckBoxItems = new List<IEnvironmentCheckBox>();
      foreach (Instance instance in args._Instances)
      {
        EnvironmentCheckBoxItems.Add(new EnvironmentCheckBox(instance.SitecoreEnvironment.Name));
      }

      if (args._SelectedEnvironments != null && args._SelectedEnvironments.Count > 0)
      {
        foreach (string selectedEnvironment in args._SelectedEnvironments)
        {
          EnvironmentCheckBoxItems.Where(item => item.Name == selectedEnvironment).FirstOrDefault().IsChecked = true;
        }
      }

      Environments.DataContext = EnvironmentCheckBoxItems;
    }
  }
}