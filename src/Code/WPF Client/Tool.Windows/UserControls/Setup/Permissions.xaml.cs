using System.Data;
using System.Data.SqlClient;
using SIM.Adapters.SqlServer;
using SIM.Base;
using SIM.Pipelines.Install;
using SIM.Tool.Base;
using SIM.Tool.Base.Wizards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows;
using SIM.Tool.Windows.Pipelines.Setup;

namespace SIM.Tool.Windows.UserControls.Setup
{
  /// <summary>
  /// Interaction logic for LocalRepository.xaml
  /// </summary>
  public partial class Permissions : IWizardStep, IFlowControl
  {
    private string _connectionString;

    public Permissions()
    {
      InitializeComponent();
    }

    protected IEnumerable<string> Accounts
    {
      get
      {
        var sqlServerAccountName = SqlServerManager.Instance.GetSqlServerAccountName(new SqlConnectionStringBuilder(_connectionString));

        if (sqlServerAccountName == null)
        {
          WindowHelper.HandleError("The instance of SQL Server cannot be reached", false);
          return null;
        }

        Log.Debug("SQL Server Account name: " + sqlServerAccountName);
        return new[] { sqlServerAccountName, Settings.CoreInstallWebServerIdentity.Value };
      }
    }

    private void Grant([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.LongRunningTask(() => this.Dispatcher.Invoke(new Action(Grant)), "Applying security changes", Window.GetWindow(this));
    }

    private void Grant()
    {
      using (new ProfileSection("Grant access", this))
      {
        var path = this.InstancesRootFolder.Text;

        if (Accounts == null) return;
        
        foreach (var account in Accounts)
        {
          if (!ValidateAccount(account))
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

    private bool ValidateAccount(string account)
    {
      if (account.Equals(@"NT SERVICE\MSSQLSERVER", StringComparison.OrdinalIgnoreCase))
      {
        var result = WindowHelper.ShowMessage("The SQL Server is configured to use \"NT SERVICE\\MSSQLSERVER\" account which is not supported by current version of SIM. You need to change the SQL Server's user account and click Grant again. The instruction will be provided when you click OK.", MessageBoxButton.OKCancel, MessageBoxImage.Error);

        if (result == MessageBoxResult.Cancel)
        {
          return false;
        }

        WindowHelper.OpenInBrowser("https://bitbucket.org/alienlab/sitecore-instance-manager/wiki/KnownIssue-SqlServerDefaultAccount", true);
        return false;
      }

      return true;
    }

    private bool Grant(string path, string accountName)
    {
      if (!FileSystem.Local.Directory.Exists(path))
      {
        WindowHelper.ShowMessage(
          "The \"{0}\" folder does not exist, please create it or return to preceding step to change it".FormatWith(path),
          MessageBoxButton.OK, MessageBoxImage.Asterisk);
        return false;
      }

      try
      {
        FileSystem.Local.Security.EnsurePermissions(path, accountName);
        return ProfileSection.Result(true);
      }
      catch (Exception ex)
      {
        Log.Error("Granting security permissions failed", this, ex);
        WindowHelper.ShowMessage("Something went wrong while assigning necessary permissions, so please do it manually according to the guide that will be opened.", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        ShowGuide();
        return ProfileSection.Result(false);
      }
    }

    private void ShowGuide()
    {
      WindowHelper.OpenInBrowser("https://bitbucket.org/alienlab/sitecore-instance-manager/wiki/Installation", true);
    }

    bool IFlowControl.OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;

      if (Accounts == null) return false;

      try
      {
        const string message = "You probably don't have necessary permissions set. Please try to click 'Grant' button before you proceed.";
        foreach (var account in Accounts)
        {
          if (!ValidateAccount(account))
          {
            return false;
          }

          if (!FileSystem.Local.Security.HasPermissions(args.InstancesRootFolderPath, account, FileSystemRights.FullControl))
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
        Log.Error("Cannot verify permissions", this, ex);
        return true;
      }
    }

    bool IFlowControl.OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      this.InstancesRootFolder.Text = args.InstancesRootFolderPath;
      _connectionString = args.ConnectionString;
    }
    
    #region IWizardStep Members


    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    #endregion
  }
}
