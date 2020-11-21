using SIM.Adapters.WebServer;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Controls;

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
      InitSiteName();
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
      args.SPSSiteName = SiteNameTextBox.Text.Trim();
      args.OverwriteExisting = OverwriteIfExistsCheckBox.IsChecked ?? false;

      string newWebroot = Path.Combine(args.SPSInstanceFolder, args.SPSSiteName);
      if (Directory.Exists(newWebroot) && !args.OverwriteExisting)
      {
        WindowHelper.ShowMessage($"{newWebroot} already exists, please delete the existing instance, choose a different name, or select the" +
          $"\"Overwrite\" checkbox to replace the existing solution");
        isValid = false;
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
        foreach (var connString in args.InstanceConnectionStrings)
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

    private void InitSiteName()
    {
      if (string.IsNullOrEmpty(SiteNameTextBox.Text.Trim()))
      {
        SiteNameTextBox.Text = "sitecore.publishing";
      }
    }

    private void InitPort()
    {
      if (string.IsNullOrEmpty(PortTextBox.Text.Trim()))
      {
        PortTextBox.Text = "80";
      }
    }

    #endregion
  }
}
