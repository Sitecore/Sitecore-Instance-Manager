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
      return;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      InstallSPSWizardArgs args = (InstallSPSWizardArgs)wizardArgs;
      args.SPSSiteName = SPSSiteNameTextBox.Text.Trim();
      args.OverwriteExisting = OverwriteIfExistsCheckBox.IsChecked ?? false;

      string newWebroot = Path.Combine(args.SPSInstanceFolder, args.SPSSiteName);
      if (Directory.Exists(newWebroot) && !args.OverwriteExisting){
        WindowHelper.ShowMessage($"{newWebroot} already exists, please delete the existing instance, choose a different name, or select the" +
          $"\"Overwrite if exists?\" checkbox to replace the existing solution");
        return false;
      }

      foreach (CheckBox checkbox in ConnectionStringsListBox.Items)
      {
        if (checkbox.IsChecked ?? false)
        {
          ConnectionString connString = args.InstanceConnectionStrings.Single(cs => cs.Name.Equals(checkbox.Content));
          args.SPSConnectionStrings.Add(checkbox.Content.ToString(), new SqlConnectionStringBuilder(connString.Value));
        }
      }

      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }
  }
}
