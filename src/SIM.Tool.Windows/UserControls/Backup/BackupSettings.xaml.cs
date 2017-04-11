namespace SIM.Tool.Windows.UserControls.Backup
{
  using System.Windows;
  using SIM.Tool.Base.Wizards;

  public partial class BackupSettings : IWizardStep, IFlowControl
  {
    #region Fields

    private bool _Databases;
    private bool _ExcludeClient;
    private bool _Files;
    private bool _MongoDatabases;

    #endregion

    #region Constructors

    public BackupSettings()
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
      var args = (BackupSettingsWizardArgs)wizardArgs;

      if (args._Databases || args._MongoDatabases || args._Files)
      {
        return true;
      }

      MessageBox.Show("You haven't chosen any backup option");
      return false;
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      BackupName.Text = ((BackupSettingsWizardArgs)wizardArgs).BackupName;
    }

    private void OnChanged(object sender, RoutedEventArgs e)
    {
      var name = ((System.Windows.Controls.CheckBox)e.Source).Name;

      switch (name)
      {
        case "Databases":
          _Databases = true;
          break;

        case "MongoDatabases":
          _MongoDatabases = true;
          break;

        case "Files":
          _Files = true;
          break;

        case "ExcludeClient":
          _ExcludeClient = true;
          _Files = true;
          Files.IsChecked = true;
          break;
      }
    }

    private void OnUnchanged(object sender, RoutedEventArgs e)
    {
      var name = ((System.Windows.Controls.CheckBox)e.Source).Name;

      switch (name)
      {
        case "Databases":
          _Databases = false;
          break;

        case "MongoDatabases":
          _MongoDatabases = false;
          break;

        case "Files":
          _Files = false;
          _ExcludeClient = false;
          ExcludeClient.IsChecked = false;
          break;

        case "ExcludeClient":
          _ExcludeClient = false;
          break;
      }
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (BackupSettingsWizardArgs)wizardArgs;

      if (!string.IsNullOrEmpty(BackupName.Text))
      {
        args.BackupName = BackupName.Text;
      }

      args._Files = _Files;
      args._Databases = _Databases;
      args._MongoDatabases = _MongoDatabases;
      args._ExcludeClient = !_ExcludeClient;

      return true;
    }

    #endregion
  }
}