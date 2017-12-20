namespace SIM.Tool.Windows.UserControls.Export
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using SIM.Tool.Base.Wizards;

  public partial class ExportDatabases : IWizardStep
  {
    #region Constructors

    public ExportDatabases()
    {
      InitializeComponent();
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var attachedDatabases = ((ExportWizardArgs)wizardArgs).Instance.AttachedDatabases.ToArray();
      Databases.DataContext = attachedDatabases.Select(database => new ExportDatabase(database.Name, true)).ToList();
    }

    private void NoneHyperlinkClick(object sender, RoutedEventArgs e)
    {
      foreach (ExportDatabase item in Databases.Items)
      {
        item.IsChecked = false;
      }
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (ExportWizardArgs)wizardArgs;
      args.SelectedDatabases = ((List<ExportDatabase>)Databases.DataContext).Where(database => database.IsChecked).Select(database => database.DatabaseName);
      if (WipeSqlServerCredentials.IsChecked != null)
      {
        args._WipeSqlServerCredentials = (bool)WipeSqlServerCredentials.IsChecked;
      }

      if (IncludeMongoDatabases.IsChecked != null)
      {
        args.IncludeMongoDatabases = (bool)IncludeMongoDatabases.IsChecked;
      }

      return true;
    }

    private void SelectAllHyperlinkClick(object sender, RoutedEventArgs e)
    {
      foreach (ExportDatabase item in Databases.Items)
      {
        item.IsChecked = true;
      }
    }

    #endregion

    #region Nested type: ExportDatabase

    private sealed class ExportDatabase : INotifyPropertyChanged
    {
      #region Delegates

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion

      #region Fields

      private bool _IsChecked;

      #endregion

      #region Constructors

      public ExportDatabase(string databaseName, bool isChecked)
      {
        DatabaseName = databaseName;
        IsChecked = isChecked;
      }

      #endregion

      #region Public properties

      public string DatabaseName { get; private set; }

      public bool IsChecked
      {
        get
        {
          return _IsChecked;
        }

        set
        {
          _IsChecked = value;
          IsCheckedPropertyChaged("IsChecked");
        }
      }

      #endregion

      #region Private methods

      private void IsCheckedPropertyChaged(string propertyName)
      {
        var handler = PropertyChanged;
        if (handler != null)
        {
          handler(this, new PropertyChangedEventArgs(propertyName));
        }
      }

      #endregion
    }

    #endregion
  }
}