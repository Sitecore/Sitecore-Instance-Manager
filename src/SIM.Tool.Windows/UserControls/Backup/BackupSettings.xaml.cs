namespace SIM.Tool.Windows.UserControls.Backup
{
  using System.Windows;
  using SIM.Tool.Base.Wizards;

  public partial class BackupSettings : IWizardStep, IFlowControl
  {
    #region Fields

    private bool _databases;
    private bool _excludeClient;
    private bool _files;
    private bool _mongoDatabases;

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

      if (args.Databases || args.MongoDatabases || args.Files)
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
          this._databases = true;
          break;

        case "MongoDatabases":
          this._mongoDatabases = true;
          break;

        case "Files":
          this._files = true;
          break;

        case "ExcludeClient":
          this._excludeClient = true;
          this._files = true;
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
          this._databases = false;
          break;

        case "MongoDatabases":
          this._mongoDatabases = false;
          break;

        case "Files":
          this._files = false;
          this._excludeClient = false;
          this.ExcludeClient.IsChecked = false;
          break;

        case "ExcludeClient":
          this._excludeClient = false;
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

      args.Files = this._files;
      args.Databases = this._databases;
      args.MongoDatabases = this._mongoDatabases;
      args.ExcludeClient = !this._excludeClient;

      return true;
    }

    #endregion
  }
}