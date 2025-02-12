namespace SIM.Tool.Windows.UserControls.Backup
{
  using System;
  using System.IO;
  using System.Windows;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;
  using JetBrains.Annotations;
  using System.Text.RegularExpressions;
  using System.Windows.Input;

  public partial class BackupFileName : IWizardStep
  {

    #region Fields

    private string _BackupName;

    #endregion

    #region Constructors

    public BackupFileName()
    {
      InitializeComponent();
      BackupName.Text = string.Format("{0:yyyy-MM-dd} at {0:hh-mm-ss}", DateTime.Now);
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      _BackupName = BackupName.Text;
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

      if (string.IsNullOrEmpty(args.BackupName))
      {
        MessageBox.Show("You haven't chosen name of backup");
        return false;
      }

      MessageBox.Show(args.BackupName);
      return true;
    }

    #endregion

    #region Private methods

    private void OnChanged(object sender, RoutedEventArgs e)
    {
      var name = ((System.Windows.Controls.TextBox)e.Source).Text;

      _BackupName = name;
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      _BackupName = BackupName.Text;

      Regex regex = new Regex("^[a-zA-Z0-9 _.-]*$");
      if (!regex.IsMatch(_BackupName))
      {
        MessageBox.Show("Invalid backup name");
        return false;
      }

      var args = (BackupWizard9Args)wizardArgs;
      args.BackupName = _BackupName;

      return true;
    }

    #endregion
  }
}