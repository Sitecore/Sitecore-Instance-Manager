using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SIM.Instances;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.UserControls.MultipleDeletion
{
  public partial class SelectInstances : IWizardStep, IFlowControl
  {
    public SelectInstances()
    {
      InitializeComponent();
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      Instances.DataContext = InstanceManager.Instances.OrderBy(instance => instance.Name);
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      var args = (MultipleDeletionWizardArgs)wizardArgs;

      if (_selectedInstances.Count != 0)
      {
        args.SelectedInstances = _selectedInstances;
      }

      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      if (_selectedInstances.Count != 0) return true;

      MessageBox.Show("You haven't selected any of the instances");
      return false;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    private void OnUnchecked(object sender, RoutedEventArgs e)
    {
      var instanceName = ((System.Windows.Controls.CheckBox) sender).Content.ToString();
      if (!string.IsNullOrEmpty(instanceName)) _selectedInstances.Remove(instanceName);
    }

    private void OnChecked(object sender, RoutedEventArgs e)
    {
      var instanceName = ((System.Windows.Controls.CheckBox)sender).Content.ToString();
      if (!string.IsNullOrEmpty(instanceName)) _selectedInstances.Add(instanceName);
    }

    private readonly List<string> _selectedInstances = new List<string>();
  }
}
