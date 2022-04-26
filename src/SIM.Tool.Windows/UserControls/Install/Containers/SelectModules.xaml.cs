using SIM.ContainerInstaller;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SIM.Tool.Windows.Dialogs;
using SIM.Tool.Base;
using SIM.Pipelines.Install.Containers;
using SIM.ContainerInstaller.Repositories.TagRepository;

namespace SIM.Tool.Windows.UserControls.Install.Containers
{
  public partial class SelectModules : IWizardStep, IFlowControl, ICustomButton
  {
    private Window owner;
    private EnvModel envModel;
    private ITagRepository tagRepository;
    private List<Module> selectedModules;
    private const string sitecoreToolsRegistry = "scr.sitecore.com/tools/";
    private const string sitecoreModuleRegistry = "scr.sitecore.com/sxp/modules/";
    private IEnumerable<string> sitecoreSpeRegistryNames = new List<string> { "sxp/modules/spe-assets", "sxp/modules/sitecore-spe-assets" };
    private const string sitecoreToolsRegistryName = "tools/sitecore-docker-tools-assets";
    private IEnumerable<string> sitecoreSxaRegistryNames = new List<string> { "sxp/modules/sxa-xm1-assets", "sxp/modules/sitecore-sxa-xm1-assets", "sxp/modules/sxa-xp1-assets", "sxp/modules/sitecore-sxa-xp1-assets" };
    private const string sitecoreToolsRegistryNamespace = "tools";
    private const string sitecoreModuleRegistryNamespace = "sxp/modules";

    public SelectModules()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;
      this.owner = args.WizardWindow;
      this.envModel = args.EnvModel;
      this.Modules.ItemsSource = new List<Module>() { Module.SXA, Module.JSS, Module.Horizon, Module.PublishingService };
      this.selectedModules = new List<Module>();
      this.tagRepository = args.TagRepository;
      this.GetToolsTags();
      this.GetSpeTags();
      this.GetSxaTags();
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      this.HideTagsControls();
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;
      args.EnvModel = this.envModel;
      args.Modules = this.selectedModules;

      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    public string CustomButtonText { get => "Advanced..."; }

    public void CustomButtonClick()
    {
      WindowHelper.ShowDialog<ContainerVariablesEditor>(this.envModel.ToList(), this.owner);
    }

    private void Module_Checked(object sender, RoutedEventArgs e)
    {
      var module = ((System.Windows.Controls.CheckBox)sender).Content;
      if (module is Module && !this.selectedModules.Contains((Module)module))
      {
        this.selectedModules.Add((Module)module);
        switch (module)
        {
          case Module.SXA:
            this.ShowOrHideToolsTagsControls(Visibility.Visible);
            this.ShowOrHideSpeAndSxaTagsControls(Visibility.Visible);
            this.envModel.SitecoreToolsRegistry = sitecoreToolsRegistry;
            this.envModel.SitecoreModuleRegistry = sitecoreModuleRegistry;
            this.envModel.ToolsVersion = this.ToolsTagsComboBox.SelectedItem.ToString();//"10.1 - 1809"
            this.envModel.SpeVersion = this.SpeTagsComboBox.SelectedItem.ToString(); // "6.2 - 1809"
            this.envModel.SxaVersion = this.SxaTagsComboBox.SelectedItem.ToString();//"10.1 - 1809"
            break;
          case Module.JSS:
            this.ShowOrHideToolsTagsControls(Visibility.Visible);
            this.ShowOrHideJssTagsControls(Visibility.Visible);
            break;
          case Module.Horizon:
            this.ShowOrHideToolsTagsControls(Visibility.Visible);
            this.ShowOrHideHorizonTagsControls(Visibility.Visible);
            break;
          case Module.PublishingService:
            this.ShowOrHideToolsTagsControls(Visibility.Visible);
            this.ShowOrHidePublishingServiceTagsControls(Visibility.Visible);
            break;
          default:
            break;
        }
      }
    }

    private void Module_Unchecked(object sender, RoutedEventArgs e)
    {
      var module = ((System.Windows.Controls.CheckBox)sender).Content;
      if (module is Module)
      {
        this.selectedModules.Remove((Module)module);
        if (this.selectedModules.Count < 1)
        {
          this.ShowOrHideToolsTagsControls(Visibility.Hidden);
        }
        switch (module)
        {
          case Module.SXA:
            this.ShowOrHideSpeAndSxaTagsControls(Visibility.Hidden);
            this.envModel.SitecoreToolsRegistry = null;
            this.envModel.SitecoreModuleRegistry = null;
            this.envModel.ToolsVersion = null;
            this.envModel.SpeVersion = null;
            this.envModel.SxaVersion = null;
            break;
          case Module.JSS:
            this.ShowOrHideJssTagsControls(Visibility.Hidden);
            break;
          case Module.Horizon:
            this.ShowOrHideHorizonTagsControls(Visibility.Hidden);
            break;
          case Module.PublishingService:
            this.ShowOrHidePublishingServiceTagsControls(Visibility.Hidden);
            break;
          default:
            break;
        }
      }
    }

    private void HideTagsControls()
    {
      this.ToolsTagsTextBlock.Visibility = Visibility.Hidden;
      this.ToolsTagsComboBox.Visibility = Visibility.Hidden;
      this.SpeTagsTextBlock.Visibility = Visibility.Hidden;
      this.SpeTagsComboBox.Visibility = Visibility.Hidden;
      this.SxaTagsTextBlock.Visibility = Visibility.Hidden;
      this.SxaTagsComboBox.Visibility = Visibility.Hidden;
      this.JssTagsTextBlock.Visibility = Visibility.Hidden;
      this.JssTagsComboBox.Visibility = Visibility.Hidden;
      this.HorizonTagsTextBlock.Visibility = Visibility.Hidden;
      this.HorizonTagsComboBox.Visibility = Visibility.Hidden;
      this.PsTagsTextBlock.Visibility = Visibility.Hidden;
      this.PsTagsComboBox.Visibility = Visibility.Hidden;
    }

    private void ShowOrHideToolsTagsControls(Visibility visibility)
    {
      this.ToolsTagsTextBlock.Visibility = visibility;
      this.ToolsTagsComboBox.Visibility = visibility;
    }

    private void ShowOrHideSpeAndSxaTagsControls(Visibility visibility)
    {
      this.SpeTagsTextBlock.Visibility = visibility;
      this.SpeTagsComboBox.Visibility = visibility;
      this.SxaTagsTextBlock.Visibility = visibility;
      this.SxaTagsComboBox.Visibility = visibility;
    }

    private void ShowOrHideJssTagsControls(Visibility visibility)
    {
      this.JssTagsTextBlock.Visibility = visibility;
      this.JssTagsComboBox.Visibility = visibility;
    }

    private void ShowOrHideHorizonTagsControls(Visibility visibility)
    {
      this.HorizonTagsTextBlock.Visibility = visibility;
      this.HorizonTagsComboBox.Visibility = visibility;
    }

    private void ShowOrHidePublishingServiceTagsControls(Visibility visibility)
    {
      this.PsTagsTextBlock.Visibility = visibility;
      this.PsTagsComboBox.Visibility = visibility;
    }

    private void ToolsTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (this.ToolsTagsComboBox.SelectedItem == null || this.selectedModules.Count < 1)
      {
        return;
      }

      this.envModel.ToolsVersion = this.ToolsTagsComboBox.SelectedItem.ToString();
    }
    private void GetToolsTags()
    {
      this.ToolsTagsComboBox.DataContext = this.tagRepository.GetToolsTags(sitecoreToolsRegistryName, sitecoreToolsRegistryNamespace).ToArray();
      this.ToolsTagsComboBox.SelectedIndex = 0;
    }

    private void SpeTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (this.SpeTagsComboBox.SelectedItem == null || !this.selectedModules.Contains(Module.SXA))
      {
        return;
      }

      this.envModel.SpeVersion = this.SpeTagsComboBox.SelectedItem.ToString();
    }

    private void GetSpeTags()
    {
      this.SpeTagsComboBox.DataContext = this.tagRepository.GetSpeOrSxaTags(sitecoreSpeRegistryNames, sitecoreModuleRegistryNamespace).ToArray();
      this.SpeTagsComboBox.SelectedIndex = 0;
    }

    private void SxaTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (this.SxaTagsComboBox.SelectedItem == null || !this.selectedModules.Contains(Module.SXA))
      {
        return;
      }

      this.envModel.SxaVersion = this.SxaTagsComboBox.SelectedItem.ToString();
    }

    private void GetSxaTags()
    {
      this.SxaTagsComboBox.DataContext = this.tagRepository.GetSpeOrSxaTags(sitecoreSxaRegistryNames, sitecoreModuleRegistryNamespace).ToArray();
      this.SxaTagsComboBox.SelectedIndex = 0;
    }
  }
}