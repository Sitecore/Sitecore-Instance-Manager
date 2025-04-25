using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.UserControls.MultipleDeletion;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SIM.Tool.Windows.UserControls.Install.SearchIndexes
{
  public partial class AvailableSearchIndexes : IWizardStep, IFlowControl
  {
    private List<IEnvironmentCheckBox> CoreNamesCheckBoxItems;

    public AvailableSearchIndexes()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      var args = (InstallSearchIndexesWizardArgs)wizardArgs;
      CoreNamesCheckBoxItems = new List<IEnvironmentCheckBox>();
      
      foreach (var availableSearchIndex in args._AvailableSearchIndexesList)
      {
        CoreNamesCheckBoxItems.Add(new EnvironmentCheckBox(availableSearchIndex));
      }

      if (args._AvailableSearchIndexesList != null && args._AvailableSearchIndexesList.Count > 0)
      {
        foreach (string availableIndexes in args._AvailableSearchIndexesList)
        {
          CoreNamesCheckBoxItems.Where(item => item.Name == availableIndexes).FirstOrDefault().IsChecked = true;
        }
      }

      SearchIndexesListBox.DataContext = CoreNamesCheckBoxItems;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      if (CoreNamesCheckBoxItems != null && CoreNamesCheckBoxItems.Where(item => item.IsChecked).Any())
      {
        return true;
      }

      MessageBox.Show("You haven't selected any of the Solr cores", "Install Solr cores", MessageBoxButton.OK, MessageBoxImage.Warning);
      return false;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      var args = (InstallSearchIndexesWizardArgs)wizardArgs;

      if (CoreNamesCheckBoxItems != null && CoreNamesCheckBoxItems.Where(item => item.IsChecked).Any())
      {
        args._AvailableSearchIndexesList = CoreNamesCheckBoxItems.Where(item => item.IsChecked).Select(item => item.Name).ToList();
      }

      return true;
    }
  }
}
