using System.Collections.Generic;
using SIM.Adapters.WebServer;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Web.Administration;

namespace SIM.Tool.Windows.UserControls.Install.PublishingService
{
  /// <summary>
  /// Interaction logic for ConfigureParameters.xaml
  /// </summary>
  public partial class ConfigureParameters : IWizardStep, IFlowControl
  {
    public ConfigureParameters()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      InstallSPSWizardArgs args = (InstallSPSWizardArgs)wizardArgs;

      InitConnectrionStringsListBox(args);
      InitSiteName(args.InstanceName);
      InitPort();
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      InstallSPSWizardArgs args = (InstallSPSWizardArgs)wizardArgs;
      bool passedValidation = true;

      ValidateAndSetSiteName(args, ref passedValidation);
      ValidateAndSetPort(args, ref passedValidation);
      SetConnectionStrings(args);

      return passedValidation;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    #region Private methods
    private void ValidateAndSetSiteName(InstallSPSWizardArgs args, ref bool isValid)
    {
      args.SPSName = SiteNameTextBox.Text.Trim();

      string newWebroot = Path.Combine(args.SPSInstanceFolder, args.SPSName);
      if (Directory.Exists(newWebroot))
      {
        WindowHelper.ShowMessage($"The webroot path '{newWebroot}' already exists.  Stop the instance using this path and uninstall it, or choose a different name");
        isValid = false;
        return;
      }

      using (ServerManager sm = new ServerManager())
      {
        if (sm.Sites.FirstOrDefault(s => s.Name.Equals(args.SPSName)) != null)
        {
          WindowHelper.ShowMessage($"The site '{args.SPSName}' already exists. Stop the instance using this site and uninstall it, or choose a different name");
          isValid = false;
        }

        else if (sm.ApplicationPools.FirstOrDefault(a => a.Name.Equals(args.SPSName)) != null)
        {
          WindowHelper.ShowMessage($"The application pool '{args.SPSName}' already exists. Stop the instance using this application pool and uninstall it, or choose a different name");
          isValid = false;
        }
      }
    }

    private void ValidateAndSetPort(InstallSPSWizardArgs args, ref bool isValid)
    {
      int port = -1;
      if(!int.TryParse(PortTextBox.Text, out port))
      {
        isValid = false;
      }
      args.SPSPort = port;
    }

    private void SetConnectionStrings(InstallSPSWizardArgs args)
    {
      args.SPSConnectionStrings.Clear();
      foreach (CheckBox checkbox in ConnectionStringsListBox.Items)
      {
        if (checkbox.IsChecked ?? false)
        {
          ConnectionString connString = args.InstanceConnectionStrings.Single(cs => cs.Name.Equals(checkbox.Content));
          args.SPSConnectionStrings.Add(checkbox.Content.ToString(), new SqlConnectionStringBuilder(connString.Value));
        }
      }
  }

    private void InitConnectrionStringsListBox(InstallSPSWizardArgs args)
    {
      if (ConnectionStringsListBox.Items.Count < 1)
      {
        foreach (var connString in GetSPSConnectionStringCandidates(args.InstanceConnectionStrings))
        {
          switch (connString.Name.ToLower().Trim())
          {
            case "core":
            case "master":
            case "web":
              ConnectionStringsListBox.Items.Add(new CheckBox() { Content = connString.Name, IsChecked = true }); break;
            default:
              ConnectionStringsListBox.Items.Add(new CheckBox() { Content = connString.Name }); break;
          }
        }
      }
    }

    private void InitSiteName(string instanceName)
    {
      if (string.IsNullOrEmpty(SiteNameTextBox.Text.Trim()))
      {
        SiteNameTextBox.Text = $"{instanceName}.publishing";
      }
    }

    private void InitPort()
    {
      if (string.IsNullOrEmpty(PortTextBox.Text.Trim()))
      {
        PortTextBox.Text = "80";
      }
    }

    private IEnumerable<ConnectionString> GetSPSConnectionStringCandidates(ConnectionStringCollection connectionStrings)
    {
      return connectionStrings.Where(cs => cs.IsSqlConnectionString && IsAllowedConnectionStringName(cs.Name));
    }

    private bool IsAllowedConnectionStringName(string name)
    {
      string[] disallowedPrefixes = new string[] { "xdb.", "exm.", "security", "messaging", "reporting", "experienceforms" };
      foreach (string prefix in disallowedPrefixes)
      {
        if (name.StartsWith(prefix))
        {
          return false;
        }
      }

      return true;
    }
    #endregion
  }
}
