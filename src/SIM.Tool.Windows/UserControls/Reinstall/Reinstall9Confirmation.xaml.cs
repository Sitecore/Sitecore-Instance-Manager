using SIM.Sitecore9Installer;
using SIM.Sitecore9Installer.Tasks;
using SIM.SitecoreEnvironments;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace SIM.Tool.Windows.UserControls.Reinstall
{
  /// <summary>
  ///   The confirm step user control.
  /// </summary>
  public partial class Reinstall9Confirmation: IWizardStep, IFlowControl
  {
    private const string PrerequisitesTaskName = "Prerequisites";

    #region Constructors

    public Reinstall9Confirmation()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      var args = wizardArgs as ReinstallWizardArgs;
      string uninstallPath = string.Empty;
      SitecoreEnvironment env = SitecoreEnvironmentHelper.GetExistingSitecoreEnvironment(args.Instance.Name);
      if (!string.IsNullOrEmpty(env?.UnInstallDataPath))
      {
        uninstallPath = env.UnInstallDataPath;
      }
      else
      {
        foreach (string installName in Directory.GetDirectories(ApplicationManager.UnInstallParamsFolder).OrderByDescending(s => s.Length))
        {
          if (args.Instance.Name.StartsWith(Path.GetFileName(installName)))
          {
            uninstallPath = installName;
            break;
          }
        }
      }

      if (string.IsNullOrEmpty(uninstallPath))
      {
        WindowHelper.ShowMessage("UnInstall files not found.");
        wizardArgs.WizardWindow.Close();
      }

      Tasker tasker = new Tasker(uninstallPath);
      InstallParam sqlServer = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlServer");
      if (sqlServer != null)
      {
        sqlServer.Value = args.ConnectionString.DataSource;
      }

      InstallParam sqlAdminUser = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlAdminUser");
      if (sqlAdminUser != null)
      {
        sqlAdminUser.Value = args.ConnectionString.UserID;
      }

      InstallParam sqlAdminPass = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlAdminPassword");
      if (sqlAdminPass != null)
      {
        sqlAdminPass.Value = args.ConnectionString.Password;
      }

      tasker.UnInstall = true;
      args.Tasker = tasker;
      StringBuilder displayText = new StringBuilder();
      displayText.AppendLine("Reinstall:");
      foreach (var task in tasker.Tasks.Where(t=>t.SupportsUninstall()))
      {
        displayText.AppendLine(string.Format(" -{0}",task.Name));
      }

      this.TextBlock.Text = displayText.ToString();
      args.InstanceName = args.Tasker.GlobalParams.First(p => p.Name == "SqlDbPrefix").Value;

      if (args.Tasker.Tasks.All(task => task.Name != PrerequisitesTaskName))
      {
        this.ReinstallPrerequisitesTextBlock.Visibility = Visibility.Hidden;
        this.ReinstallPrerequisitesCheckBox.Visibility = Visibility.Hidden;
      }
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      ReinstallWizardArgs args = (ReinstallWizardArgs)wizardArgs;
      if (this.ReinstallPrerequisitesCheckBox.Visibility == Visibility.Visible && 
          this.ReinstallPrerequisitesCheckBox.IsChecked != null && 
          !this.ReinstallPrerequisitesCheckBox.IsChecked.Value)
      {
        foreach (Task task in args.Tasker.Tasks)
        {
          if (task.Name == PrerequisitesTaskName)
          {
            task.ShouldRun = false;
          }
        }
      }

      return true;      
    }

    

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }



    #endregion
  }
}