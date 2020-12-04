using ContainerInstaller;
using SIM.Sitecore9Installer;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using SIM.Tool.Windows.Dialogs;
using TaskDialogInterop;

namespace SIM.Tool.Windows.UserControls.Install.Containers
{
  /// <summary>
  /// Interaction logic for Instance9SelectTasks.xaml
  /// </summary>
  public partial class SelectTag : IWizardStep, IFlowControl, ICustomButton
  {
    private Window owner;
    private string productVersion;
    private string lastRegistry;
    private EnvModel envModel;
    public SelectTag()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;
      this.owner = args.WizardWindow;
      this.productVersion = args.Product.TriVersion;      
      string[] envFiles = Directory.GetFiles(args.FilesRoot, ".env", SearchOption.AllDirectories);
      string topologiesFolder = Directory.GetParent(envFiles[0]).Parent.FullName;
      this.Topoligies.DataContext = Directory.GetDirectories(topologiesFolder).Select(d => new NameValueModel(Path.GetFileName(d), d));
      this.Topoligies.SelectedIndex = 0;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;
      args.Tag = (string)this.Tags.SelectedValue;
      args.DockerRoot = ((NameValueModel)this.Topoligies.SelectedItem).Value;
      args.EnvModel = this.envModel;
      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    } 
    
    private string[] GetTags(string productVersion, string tagNameSpace)
    {
      return new string[] { "10.0.0-1909", "10.0.0-ltsc2019", "some random tag"  };
    }

    private class NameValueModel
    {
      public NameValueModel(string name, string value)
      {
        this.Name = name;
        this.Value = value;
      }

      public string Name { get; }
      public string Value { get; }
    }

    private void Topoligies_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (this.Topoligies.SelectedItem == null)
      {
        return;
      }

      NameValueModel topology = (NameValueModel)this.Topoligies.SelectedItem;
      string envPath = Path.Combine(topology.Value, ".env");
      EnvModel model = EnvModel.LoadFromFile(envPath);
      this.envModel = model;
      if (this.lastRegistry == model.SitecoreRegistry)
      {
        return;
      }

      this.lastRegistry = model.SitecoreRegistry;
      Uri registry = new Uri("https://" + model.SitecoreRegistry, UriKind.Absolute);
      this.Tags.DataContext = this.GetTags(this.productVersion, registry.LocalPath.Trim('/'));
      this.Tags.SelectedIndex = 0;
    }

    private void Tags_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (this.Tags.SelectedItem == null)
      {
        return;
      }

      string tag = (string)this.Tags.SelectedItem;
      this.envModel.SitecoreVersion = tag;
    }

    public string CustomButtonText { get => "Advanced..."; }

    public void CustomButtonClick()
    {
      WindowHelper.ShowDialog<ContainerVariablesEditor>(this.envModel.ToList(), this.owner);
    }
  }
}