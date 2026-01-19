namespace SIM.Tool.Windows.UserControls.Backup
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using SIM.Tool.Base.Wizards;

  public partial class BackupDatabases : IWizardStep
  {
    #region Constructors

    public BackupDatabases()
    {
      InitializeComponent();
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var attachedDatabases = ((BackupWizard9Args)wizardArgs).Instance.AttachedDatabases.ToArray();
      Databases.DataContext = attachedDatabases.Select(database => new BackupDatabase(database.Name, true)).ToList();
    }

    private void NoneHyperlinkClick(object sender, RoutedEventArgs e)
    {
      foreach (BackupDatabase item in Databases.Items)
      {
        item.IsChecked = false;
      }
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (BackupWizard9Args)wizardArgs;
      args.SelectedDatabases = ((List<BackupDatabase>)Databases.DataContext).Where(database => database.IsChecked).Select(database => database.DatabaseName);

      if (args.SelectedDatabases.Count<string>() > 0) args._BackupDatabase = true;
      else args._BackupDatabase = false;

      return true;
    }

    private void SelectAllHyperlinkClick(object sender, RoutedEventArgs e)
    {
      foreach (BackupDatabase item in Databases.Items)
      {
        item.IsChecked = true;
      }
    }

    #endregion

    #region Nested type: BackupDatabase

    private sealed class BackupDatabase : INotifyPropertyChanged
    {
      #region Delegates

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion

      #region Fields

      private bool _IsChecked;

      #endregion

      #region Constructors

      public BackupDatabase(string databaseName, bool isChecked)
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