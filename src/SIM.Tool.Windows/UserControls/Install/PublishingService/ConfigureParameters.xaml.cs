using SIM.Adapters.WebServer;
using SIM.Extensions;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SIM.Tool.Windows.UserControls.Install.PublishingService
{
  /// <summary>
  /// Interaction logic for ConfigureParameters.xaml
  /// </summary>
  public partial class ConfigureParameters : IWizardStep, IFlowControl
  {

    public ConnectionStringCollection ConnectionStrings { get; set; }

    public ConfigureParameters()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      InstallPublishingServiceWizardArgs args = (InstallPublishingServiceWizardArgs)wizardArgs;
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
      return;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      InstallPublishingServiceWizardArgs args = (InstallPublishingServiceWizardArgs)wizardArgs;
      args.PublishingServiceSiteName = PublishingServiceSiteNameTextBox.Text.Trim();

      string newWebroot = Path.Combine(args.PublishingServiceInstanceFolder, args.PublishingServiceSiteName);
      if (Directory.Exists(newWebroot)){
        WindowHelper.ShowMessage($"{newWebroot} already exists, please remove it or choose a different name for your instance");
        return false;
      }

      foreach (CheckBox checkbox in ConnectionStringsListBox.Items)
      {
        if (checkbox.IsChecked ?? false)
        {
          ConnectionString connString = args.InstanceConnectionStrings.Single(cs => cs.Name.Equals(checkbox.Content));
          args.PublishingServiceConnectionStrings.Add(checkbox.Content.ToString(), new SqlConnectionStringBuilder(connString.Value));
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
