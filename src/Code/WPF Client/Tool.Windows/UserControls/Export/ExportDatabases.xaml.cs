namespace SIM.Tool.Windows.UserControls.Export
{
  using System.ComponentModel;
  using System.Linq;
  using System.Collections.Generic;
  using System.Windows;
  using Base.Wizards;

  public partial class ExportDatabases : IWizardStep
  {
    public ExportDatabases()
    {
      InitializeComponent();
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var attachedDatabases = ((ExportWizardArgs)wizardArgs).Instance.AttachedDatabases.ToArray();
      Databases.DataContext = attachedDatabases.Select(database => new ExportDatabase(database.Name, true)).ToList();
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {                 
      var args = (ExportWizardArgs)wizardArgs;
      args.SelectedDatabases = ((List<ExportDatabase>)Databases.DataContext).Where(database => database.IsChecked).Select(database => database.DatabaseName);
      if (WipeSqlServerCredentials.IsChecked != null) args.WipeSqlServerCredentials = (bool) WipeSqlServerCredentials.IsChecked;
      if (IncludeMongoDatabases.IsChecked != null) args.IncludeMongoDatabases = (bool)IncludeMongoDatabases.IsChecked;
      return true;
    }

    private void SelectAllHyperlinkClick(object sender, RoutedEventArgs e)
    {
      foreach (ExportDatabase item in Databases.Items)
      {
        item.IsChecked = true;
      }
    }

    private void NoneHyperlinkClick(object sender, RoutedEventArgs e)
    {
      foreach (ExportDatabase item in Databases.Items)
      {
        item.IsChecked = false;
      }
    }

    private sealed class ExportDatabase : INotifyPropertyChanged
    {
      private bool _isChecked;

      public ExportDatabase(string databaseName, bool isChecked)
      {
        DatabaseName = databaseName;
        IsChecked = isChecked;
      }

      public bool IsChecked
      {
        get
        {
          return _isChecked;
        }

        set
        {
          _isChecked = value;
          IsCheckedPropertyChaged("IsChecked");
        }
      }
      public string DatabaseName { get; private set; }
      public event PropertyChangedEventHandler PropertyChanged;

      private void IsCheckedPropertyChaged(string propertyName)
      {
        var handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
