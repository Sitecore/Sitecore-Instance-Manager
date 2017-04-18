using System.IO;

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
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Core;
  using SIM.Extensions;

  public partial class Permissions : IWizardStep, IFlowControl
  {
    #region Fields

    private string _ConnectionString;

    #endregion

    #region Constructors

    public Permissions()
    {
      InitializeComponent();
    }

    #endregion

    #region Protected properties

    protected IEnumerable<string> Accounts
    {
      get
      {
        var sqlServerAccountName = SqlServerManager.Instance.GetSqlServerAccountName(new SqlConnectionStringBuilder(_ConnectionString));

        if (sqlServerAccountName == null)
        {
          WindowHelper.HandleError("The instance of SQL Server cannot be reached", false);
          return null;
        }

        Log.Debug($"SQL Server Account name: {sqlServerAccountName}");
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
      WindowHelper.LongRunningTask(() => Dispatcher.Invoke(Grant), "Applying security changes", Window.GetWindow(this));
    }

    private void Grant()
    {
      using (new ProfileSection("Grant access", this))
      {
        var path = InstancesRootFolder.Text;

        if (Accounts == null)
        {
          return;
        }

        foreach (var account in Accounts)
        {
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
        WindowHelper.ShowMessage($"Something went wrong while assigning necessary permissions, so please assign them manually: grant the \"{path}\" folder with FULL ACCESS rights for {accountName} user account.", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        return ProfileSection.Result(false);
      }
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      InstancesRootFolder.Text = args.InstancesRootFolderPath;
      _ConnectionString = args.ConnectionString;
    }

    bool IFlowControl.OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    bool IFlowControl.OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;

      if (Accounts == null)
      {
        return false;
      }

      try
      {
        const string Message = "You probably don't have necessary permissions set. Please try to click 'Grant' button before you proceed.\r\n\r\nNote, the SQL Server account that you selected previously must have necessary permissions to create a SQL database in the instances root folder you specified earlier - please ensure that it is correct. In addition, the SQL Server service must use NETWORK SERVICE identity so that SIM can assign necessary permissions for it.";
        foreach (var account in Accounts)
        {
          if (account.StartsWith("NT Service", StringComparison.OrdinalIgnoreCase))
          {
            if (WindowHelper.ShowMessage(
                  $"Cannot check if {account} has Full Access permissions for the {args.InstancesRootFolderPath} folder. Please check that manually and click YES.",
                  MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
            {
              return false;
            }

            continue;
          }

          if (!FileSystem.FileSystem.Local.Security.HasPermissions(args.InstancesRootFolderPath, account, FileSystemRights.FullControl))
          {
            WindowHelper.ShowMessage(Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            return false;
          }
        }

        if (!SqlServerManager.Instance.TestSqlServer(args.InstancesRootFolderPath, args.ConnectionString))
        {
          WindowHelper.ShowMessage(Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
          return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Cannot verify permissions");
        WindowHelper.ShowMessage($"Cannot verify permissions\r\nException: {ex.GetType()}\r\nMessage: {ex.Message}\r\nStackTrace:\r\n{ex.StackTrace}");

        return false;
      }
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
