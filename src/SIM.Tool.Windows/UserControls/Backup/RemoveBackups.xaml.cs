namespace SIM.Tool.Windows.UserControls.Backup
{
  using System.Collections.Generic;
  using SIM.Adapters.SqlServer;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;
  using System.ComponentModel;
  using System.Linq;
  using Sitecore.Diagnostics.Base;

  #region

  public partial class RemoveBackups : IWizardStep
  {
    #region Constructors

    public RemoveBackups()
    {
      InitializeComponent();
    }

    #endregion

    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var attachedBackupes = ((RemoveBackupsWizardArgs)wizardArgs).Instance.Backups.ToArray();
      Backups.DataContext = attachedBackupes.Select(backup => new BackupFolder(backup.FolderPath, false)).ToList();
    }

    private void SelectAllHyperlinkClick(object sender, RoutedEventArgs e)
    {
      foreach (BackupFolder item in Backups.Items)
      {
        item.IsChecked = true;
      }
    }

    private void NoneHyperlinkClick(object sender, RoutedEventArgs e)
    {
      foreach (BackupFolder item in Backups.Items)
      {
        item.IsChecked = false;
      }
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (RemoveBackupsWizardArgs)wizardArgs;
      var _backups = ((List<BackupFolder>)Backups.DataContext).Where(backup => backup.IsChecked).Select(backup => backup.FolderPath);

      if (!_backups.Any())
      {
        MessageBox.Show("Any backup wasn\'t chosen");
        return false;
      }

      args.Backups = _backups;
      return true;
    }

    #endregion

    #region Nested type: BackupFolder

    private sealed class BackupFolder : INotifyPropertyChanged
    {
      #region Delegates

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion

      #region Fields

      private bool _IsChecked;

      #endregion

      #region Constructors

      public BackupFolder(string folder, bool isChecked)
      {
        FolderPath = folder;
        IsChecked = isChecked;
      }

      #endregion

      #region Public properties

      public string FolderPath { get; private set; }

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
  #endregion
}