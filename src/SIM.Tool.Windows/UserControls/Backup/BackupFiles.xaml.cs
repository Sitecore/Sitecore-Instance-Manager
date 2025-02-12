namespace SIM.Tool.Windows.UserControls.Backup
{
  using System;
  using System.IO;
  using System.Windows;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;
  using JetBrains.Annotations;

  public partial class BackupFiles : IWizardStep
  {

    #region Fields

    private bool _ExcludeClient;
    private bool _Files;

    #endregion


    #region Constructors

    public BackupFiles()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Files.IsChecked = false;
      ExcludeClient.IsChecked = false;
      _Files = false;
    }

    #endregion

    #region Public methods

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (BackupWizard9Args)wizardArgs;

      if ((args._BackupDatabase) || (args._Files))
      {
        return true;
      }

      MessageBox.Show("You haven't chosen any backup option");
      return false;
    }

    #endregion


    #region Private methods

    private void OnChanged(object sender, RoutedEventArgs e)
    {
      var name = ((System.Windows.Controls.CheckBox)e.Source).Name;

      switch (name)
      {
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
      var args = (BackupWizard9Args)wizardArgs;

      args._Files = _Files;
      args._BackupClient = !_ExcludeClient;

      if (args._BackupDatabase || args._Files)
      {
        return true;
      }

      MessageBox.Show("You haven't chosen any backup option");
      return false;
    }

    #endregion
  }
}