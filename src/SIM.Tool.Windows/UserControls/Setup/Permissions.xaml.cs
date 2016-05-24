namespace SIM.Tool.Windows.UserControls.Setup
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.Security.AccessControl;
  using System.Windows;
  using SIM.Adapters.SqlServer;
  using SIM.Pipelines.Install;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.Pipelines.Setup;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Core;

  public partial class Permissions : IWizardStep, IFlowControl
  {
    #region Fields

    private string _connectionString;

    #endregion

    #region Constructors

    public Permissions()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Protected properties

    protected IEnumerable<string> Accounts
    {
      get
      {
        var sqlServerAccountName = SqlServerManager.Instance.GetSqlServerAccountName(new SqlConnectionStringBuilder(this._connectionString));

        if (sqlServerAccountName == null)
        {
          WindowHelper.HandleError("The instance of SQL Server cannot be reached", false);
          return null;
        }

        Log.Debug("SQL Server Account name: {0}",  sqlServerAccountName);
        return new[]
        {
          sqlServerAccountName, Settings.CoreInstallWebServerIdentity.Value
        };
      }
    }

    #endregion

    #region Private methods

    private void Grant([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.LongRunningTask(() => this.Dispatcher.Invoke(new Action(this.Grant)), "Applying security changes", Window.GetWindow(this));
    }

    private void Grant()
    {
      using (new ProfileSection("Grant access", this))
      {
        var path = this.InstancesRootFolder.Text;

        if (this.Accounts == null)
        {
          return;
        }

        foreach (var account in this.Accounts)
        {
          if (!this.ValidateAccount(account))
          {
            return;
          }

          if (!Grant(path, account))
          {
            return;
          }
        }

        WindowHelper.ShowMessage("Permissions were successfully set", MessageBoxButton.OK, MessageBoxImage.Information);

        ProfileSection.Result("Done.");
      }
    }

    private bool Grant(string path, string accountName)
    {
      if (!FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        WindowHelper.ShowMessage(
          "The \"{0}\" folder does not exist, please create it or return to preceding step to change it".FormatWith(path), 
          MessageBoxButton.OK, MessageBoxImage.Asterisk);
        return false;
      }

      try
      {
        FileSystem.FileSystem.Local.Security.EnsurePermissions(path, accountName);
        return ProfileSection.Result(true);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Granting security permissions failed");
        WindowHelper.ShowMessage(string.Format("Something went wrong while assigning necessary permissions, so please assign them manually: grant the \"{0}\" folder with FULL ACCESS rights for {1} user account.", path, accountName), MessageBoxButton.OK, MessageBoxImage.Asterisk);
        return ProfileSection.Result(false);
      }
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      this.InstancesRootFolder.Text = args.InstancesRootFolderPath;
      this._connectionString = args.ConnectionString;
    }

    bool IFlowControl.OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    bool IFlowControl.OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;

      if (this.Accounts == null)
      {
        return false;
      }

      try
      {
        const string message = "You probably don't have necessary permissions set. Please try to click 'Grant' button before you proceed.";
        foreach (var account in this.Accounts)
        {
          if (!this.ValidateAccount(account))
          {
            return false;
          }

          if (!FileSystem.FileSystem.Local.Security.HasPermissions(args.InstancesRootFolderPath, account, FileSystemRights.FullControl))
          {
            WindowHelper.ShowMessage(message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            return false;
          }
        }

        if (!SqlServerManager.Instance.TestSqlServer(args.InstancesRootFolderPath, args.ConnectionString))
        {
          WindowHelper.ShowMessage(message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
          return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Cannot verify permissions");
        return true;
      }
    }

    private bool ValidateAccount(string account)
    {
      if (account.Equals(@"NT SERVICE\MSSQLSERVER", StringComparison.OrdinalIgnoreCase))
      {
        var result = WindowHelper.ShowMessage("The SQL Server is configured to use \"NT SERVICE\\MSSQLSERVER\" account which is not supported by current version of SIM. You need to change the SQL Server's user account and click Grant again. The instruction will be provided when you click OK.", MessageBoxButton.OKCancel, MessageBoxImage.Error);

        if (result == MessageBoxResult.Cancel)
        {
          return false;
        }

        CoreApp.OpenInBrowser("https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Troubleshooting", true);
        return false;
      }

      return true;
    }

    #endregion

    #region IWizardStep Members

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    #endregion
  }
}