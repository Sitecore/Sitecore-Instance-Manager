using SIM.ContainerInstaller;
using SIM.Core;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using SIM.ContainerInstaller.Repositories.TagRepository;

namespace SIM.Tool.Windows.UserControls.Install.Containers
{
  /// <summary>
  /// Interaction logic for Instance9SelectTasks.xaml
  /// </summary>
  public partial class SelectTag : IWizardStep, IFlowControl
  {
    private Window owner;
    private string productVersion;
    private string lastRegistry;
    private EnvModel envModel;
    private readonly ITagRepository tagRepository;

    public SelectTag()
    {
      InitializeComponent();
      this.tagRepository = GitHubTagRepository.GetInstance();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;
      this.owner = args.WizardWindow;
      this.productVersion = args.Product.TriVersion;      
      string[] envFiles = Directory.GetFiles(args.FilesRoot, ".env", SearchOption.AllDirectories);
      string topologiesFolder = Directory.GetParent(envFiles[0]).Parent.FullName;
      this.Topologies.DataContext = Directory.GetDirectories(topologiesFolder).Select(d => new NameValueModel(Path.GetFileName(d), d));
      this.Topologies.SelectedIndex = 0;
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
      args.DockerRoot = ((NameValueModel)this.Topologies.SelectedItem).Value;
      this.envModel.SitecoreVersion = args.Tag;
      this.envModel.ProjectName = args.InstanceName;
      args.EnvModel = this.envModel;
      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    } 
    
    private string[] GetTags(string productVersion, string tagNameSpace)
    {
      return this.tagRepository.GetSortedShortSitecoreTags(productVersion, tagNameSpace).ToArray(); ;
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
      if (this.Topologies.SelectedItem == null)
      {
        return;
      }

      NameValueModel topology = (NameValueModel)this.Topologies.SelectedItem;
      string envPath = Path.Combine(topology.Value, ".env");
      this.envModel = this.CreateModel(envPath);
      if (this.lastRegistry == this.envModel.SitecoreRegistry)
      {
        return;
      }

      this.lastRegistry = this.envModel.SitecoreRegistry;
      Uri registry = new Uri("https://" + this.envModel.SitecoreRegistry, UriKind.Absolute);
      this.Tags.DataContext = this.GetTags(this.productVersion, registry.LocalPath.Trim('/'));
      this.Tags.SelectedIndex = 0;
    }

    private EnvModel CreateModel(string envPath)
    {
      EnvModel model = EnvModel.LoadFromFile(envPath);
      if (string.IsNullOrWhiteSpace(model.SqlAdminPassword))
      {
        model.SqlAdminPassword = ProfileManager.Profile.SqlPassword;
      }

      if (string.IsNullOrWhiteSpace(model.SitecoreAdminPassword))
      {
        model.SitecoreAdminPassword = CoreAppSettings.AppLoginAsAdminNewPassword.Value;
      }

      return model;
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
  }

 
}
