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
      this.InitializeComponent();
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
      this.BackupName.Text = ((BackupSettingsWizardArgs)wizardArgs).BackupName;
    }

    private void OnChanged(object sender, RoutedEventArgs e)
    {
      var name = ((System.Windows.Controls.CheckBox)e.Source).Name;

      switch (name)
      {
        case "Databases":
          this._Databases = true;
          break;

        case "MongoDatabases":
          this._MongoDatabases = true;
          break;

        case "Files":
          this._Files = true;
          break;

        case "ExcludeClient":
          this._ExcludeClient = true;
          this._Files = true;
          this.Files.IsChecked = true;
          break;
      }
    }

    private void OnUnchanged(object sender, RoutedEventArgs e)
    {
      var name = ((System.Windows.Controls.CheckBox)e.Source).Name;

      switch (name)
      {
        case "Databases":
          this._Databases = false;
          break;

        case "MongoDatabases":
          this._MongoDatabases = false;
          break;

        case "Files":
          this._Files = false;
          this._ExcludeClient = false;
          this.ExcludeClient.IsChecked = false;
          break;

        case "ExcludeClient":
          this._ExcludeClient = false;
          break;
      }
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (BackupSettingsWizardArgs)wizardArgs;

      if (!string.IsNullOrEmpty(this.BackupName.Text))
      {
        args.BackupName = this.BackupName.Text;
      }

      args._Files = this._Files;
      args._Databases = this._Databases;
      args._MongoDatabases = this._MongoDatabases;
      args._ExcludeClient = !this._ExcludeClient;

      return true;
    }

    #endregion
  }
}