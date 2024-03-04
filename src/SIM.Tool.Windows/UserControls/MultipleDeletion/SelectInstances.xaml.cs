namespace SIM.Tool.Windows.UserControls.MultipleDeletion
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using SIM.Extensions;
  using SIM.Instances;
  using SIM.Tool.Base.Wizards;

  public partial class SelectInstances : IWizardStep, IFlowControl
  {
    #region Fields

    private List<IEnvironmentCheckBox> InstanceCheckBoxItems;
    private List<IEnvironmentCheckBox> SearchInstanceCheckBoxItems;

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
      if (InstanceCheckBoxItems != null && InstanceCheckBoxItems.Where(item => item.IsChecked).Any())
      {
        return true;
      }

      MessageBox.Show("You haven't selected any of the instances", "Multiple deletion Sitecore 8 and earlier", MessageBoxButton.OK, MessageBoxImage.Warning);
      return false;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      if (InstanceCheckBoxItems != null && InstanceCheckBoxItems.Where(item => item.IsChecked).Any())
      {
        var args = (MultipleDeletionWizardArgs)wizardArgs;
        args._SelectedInstances = InstanceCheckBoxItems.Where(item => item.IsChecked).Select(item => item.Name).ToList();
      }

      return true;
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      if (InstanceCheckBoxItems == null)
      {
        var args = (MultipleDeletionWizardArgs)wizardArgs;

        InstanceCheckBoxItems = new List<IEnvironmentCheckBox>();
        foreach (Instance instance in args._Instances)
        {
          InstanceCheckBoxItems.Add(new EnvironmentCheckBox(instance.Name));
        }

        Instances.DataContext = InstanceCheckBoxItems;
      }
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
      InstanceCheckBoxItems.ForEach(item => item.IsChecked = false);

      string searchPhrase = SearchTextBox.Text.Trim();

      if (!string.IsNullOrEmpty(searchPhrase))
      {
        SearchInstanceCheckBoxItems = InstanceCheckBoxItems.Where(item => item.Name.ContainsIgnoreCase(searchPhrase)).ToList();
        Instances.DataContext = SearchInstanceCheckBoxItems;
      }
      else
      {
        Instances.DataContext = InstanceCheckBoxItems;
      }
    }

    #endregion
  }
}