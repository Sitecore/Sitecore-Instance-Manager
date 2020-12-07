using ContainerInstaller;
using SIM.Core;
using SIM.Sitecore9Installer;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using SIM.Tool.Windows.Dialogs;
using ContainerInstaller.Repositories.TagRepository;
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
    private readonly ITagRepository tagRepository;
    private string defaultProjectName;

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
      this.Topoligies.DataContext = Directory.GetDirectories(topologiesFolder).Select(d => new NameValueModel(Path.GetFileName(d), d));
      this.Topoligies.SelectedIndex = 0;
      this.defaultProjectName = args.InstanceName;
      this.ProjectName.IsChecked = true;
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
      this.envModel.SitecoreVersion = args.Tag;
      args.EnvModel = this.envModel;
      args.Topology = ((NameValueModel)this.Topoligies.SelectedItem).Name.ToString();

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
      if (this.Topoligies.SelectedItem == null)
      {
        return;
      }

      NameValueModel topology = (NameValueModel)this.Topoligies.SelectedItem;
      string envPath = Path.Combine(topology.Value, ".env");
      this.envModel = this.CreateModel(envPath);
      if (this.lastRegistry == this.envModel.SitecoreRegistry)
      {
        this.UpdateProjectName();
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

      if (string.IsNullOrWhiteSpace(model.SitecoreLicense))
      {
        model.SitecoreLicense = ProfileManager.Profile.License;
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
      this.UpdateProjectName();
    }

    private void ProjectName_Checked(object sender, RoutedEventArgs e)
    {
      this.UpdateProjectName();
    }

    private void ProjectName_OnUnchecked(object sender, RoutedEventArgs e)
    {
      this.envModel.ProjectName = this.defaultProjectName;
    }

    private void UpdateProjectName()
    {
      if (ProjectName.IsChecked == true && this.Topoligies.SelectedItem != null && this.Tags.SelectedItem != null)
      {
        this.envModel.ProjectName =
          $"{this.defaultProjectName}-{((NameValueModel)this.Topoligies.SelectedItem).Name}-{this.Tags.SelectedItem}";
      }
    }
      
    public string CustomButtonText { get => "Advanced..."; }

    public void CustomButtonClick()
    {
      WindowHelper.ShowDialog<ContainerVariablesEditor>(this.envModel.ToList(), this.owner);
      this.UpdateTagsControl(this.envModel.SitecoreVersion);
    }

    private void UpdateTagsControl(string tag)
    {
      if (this.Topoligies.SelectedItem != null && !string.Equals((string)this.Tags.SelectedItem, tag, StringComparison.InvariantCultureIgnoreCase))
      {
        foreach (string item in this.Tags.ItemsSource)
        {
          if (string.Equals(item, tag, StringComparison.InvariantCultureIgnoreCase))
          {
            this.Tags.SelectedItem = item;
            return;
          }
        }

        List<string> tagsItems = new List<string>(this.Tags.ItemsSource as IEnumerable<string>)
        {
          tag
        };
        this.Tags.DataContext = tagsItems;
        this.Tags.SelectedIndex = this.Tags.Items.Count - 1;
      }
    }
  }
}