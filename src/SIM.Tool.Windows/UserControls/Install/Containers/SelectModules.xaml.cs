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
    private const string SitecoreContainerRegistryHost = "scr.sitecore.com";
    private const string SitecoreToolsRegistryNamespace = "tools";
    private const string SitecoreModuleRegistryNamespace = "sxp/modules";
    private string SitecoreToolsRegistryName = $"{SitecoreToolsRegistryNamespace}/sitecore-docker-tools-assets";
    private string SpeRegistryName = $"{SitecoreModuleRegistryNamespace}/spe-assets";
    private string SitecoreSpeRegistryName = $"{SitecoreModuleRegistryNamespace}/sitecore-spe-assets";
    private string SxaRegistryNameXm1 = $"{SitecoreModuleRegistryNamespace}/sxa-xm1-assets";
    private string SitecoreSxaRegistryNameXm1 = $"{SitecoreModuleRegistryNamespace}/sitecore-sxa-xm1-assets";
    private string SxaRegistryNameXp = $"{SitecoreModuleRegistryNamespace}/sxa-xp1-assets";
    private string SitecoreSxaRegistryNameXp = $"{SitecoreModuleRegistryNamespace}/sitecore-sxa-xp1-assets";

    public SelectModules()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;
      owner = args.WizardWindow;
      envModel = args.EnvModel;
      HideTagsControls();
      Modules.ItemsSource = new List<Module>() { Module.SXA }; // Module.JSS, Module.Horizon, Module.PublishingService
      selectedModules = new List<Module>();
      tagRepository = args.TagRepository;
      GetToolsTags();
      GetSpeAndSxaTags(args.Product.ShortVersion, args.Topology);
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      HideTagsControls();
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;
      args.EnvModel = envModel;
      args.Modules = selectedModules;

      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    public string CustomButtonText { get => "Advanced..."; }

    public void CustomButtonClick()
    {
      WindowHelper.ShowDialog<ContainerVariablesEditor>(envModel.ToList(), owner);
    }

    private void Module_Checked(object sender, RoutedEventArgs e)
    {
      var module = ((System.Windows.Controls.CheckBox)sender).Content;
      if (module is Module && !selectedModules.Contains((Module)module))
      {
        selectedModules.Add((Module)module);

        ShowOrHideToolsTagsControls(Visibility.Visible);

        envModel.SitecoreToolsRegistry = $"{SitecoreContainerRegistryHost}/{SitecoreToolsRegistryNamespace}/";
        envModel.SitecoreModuleRegistry = $"{SitecoreContainerRegistryHost}/{SitecoreModuleRegistryNamespace}/";
        envModel.ToolsVersion = ToolsTagsComboBox.SelectedItem.ToString();

        switch (module)
        {
          case Module.SXA:
            ShowOrHideSpeAndSxaTagsControls(Visibility.Visible);
            envModel.SpeVersion = SpeTagsComboBox.SelectedItem.ToString();
            envModel.SxaVersion = SxaTagsComboBox.SelectedItem.ToString();
            break;
          case Module.JSS:
            ShowOrHideJssTagsControls(Visibility.Visible);
            break;
          case Module.Horizon:
            ShowOrHideHorizonTagsControls(Visibility.Visible);
            break;
          case Module.PublishingService:
            ShowOrHidePublishingServiceTagsControls(Visibility.Visible);
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
        selectedModules.Remove((Module)module);
        if (selectedModules.Count < 1)
        {
          ShowOrHideToolsTagsControls(Visibility.Collapsed);
        }
        switch (module)
        {
          case Module.SXA:
            ShowOrHideSpeAndSxaTagsControls(Visibility.Collapsed);
            envModel.Remove(EnvModel.SitecoreToolsRegistryName);
            envModel.Remove(EnvModel.SitecoreModuleRegistryName);
            envModel.Remove(EnvModel.ToolsVersionName);
            envModel.Remove(EnvModel.SpeVersionName);
            envModel.Remove(EnvModel.SxaVersionName);
            break;
          case Module.JSS:
            ShowOrHideJssTagsControls(Visibility.Collapsed);
            break;
          case Module.Horizon:
            ShowOrHideHorizonTagsControls(Visibility.Collapsed);
            break;
          case Module.PublishingService:
            ShowOrHidePublishingServiceTagsControls(Visibility.Collapsed);
            break;
          default:
            break;
        }
      }
    }

    private void HideTagsControls()
    {
      ToolsTagsTextBlock.Visibility = Visibility.Collapsed;
      ToolsTagsComboBox.Visibility = Visibility.Collapsed;
      SpeTagsTextBlock.Visibility = Visibility.Collapsed;
      SpeTagsComboBox.Visibility = Visibility.Collapsed;
      SxaTagsTextBlock.Visibility = Visibility.Collapsed;
      SxaTagsComboBox.Visibility = Visibility.Collapsed;
      JssTagsTextBlock.Visibility = Visibility.Collapsed;
      JssTagsComboBox.Visibility = Visibility.Collapsed;
      HorizonTagsTextBlock.Visibility = Visibility.Collapsed;
      HorizonTagsComboBox.Visibility = Visibility.Collapsed;
      PsTagsTextBlock.Visibility = Visibility.Collapsed;
      PsTagsComboBox.Visibility = Visibility.Collapsed;
    }

    private void ShowOrHideToolsTagsControls(Visibility visibility)
    {
      ToolsTagsTextBlock.Visibility = visibility;
      ToolsTagsComboBox.Visibility = visibility;
    }

    private void ShowOrHideSpeAndSxaTagsControls(Visibility visibility)
    {
      SpeTagsTextBlock.Visibility = visibility;
      SpeTagsComboBox.Visibility = visibility;
      SxaTagsTextBlock.Visibility = visibility;
      SxaTagsComboBox.Visibility = visibility;
    }

    private void ShowOrHideJssTagsControls(Visibility visibility)
    {
      JssTagsTextBlock.Visibility = visibility;
      JssTagsComboBox.Visibility = visibility;
    }

    private void ShowOrHideHorizonTagsControls(Visibility visibility)
    {
      HorizonTagsTextBlock.Visibility = visibility;
      HorizonTagsComboBox.Visibility = visibility;
    }

    private void ShowOrHidePublishingServiceTagsControls(Visibility visibility)
    {
      PsTagsTextBlock.Visibility = visibility;
      PsTagsComboBox.Visibility = visibility;
    }

    private void ToolsTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (ToolsTagsComboBox.SelectedItem == null || selectedModules.Count < 1)
      {
        return;
      }

      envModel.ToolsVersion = ToolsTagsComboBox.SelectedItem.ToString();
    }

    private void SpeTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (SpeTagsComboBox.SelectedItem == null || !selectedModules.Contains(Module.SXA))
      {
        return;
      }

      envModel.SpeVersion = SpeTagsComboBox.SelectedItem.ToString();
    }

    private void SxaTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (SxaTagsComboBox.SelectedItem == null || !selectedModules.Contains(Module.SXA))
      {
        return;
      }

      envModel.SxaVersion = SxaTagsComboBox.SelectedItem.ToString();
    }

    private void GetToolsTags()
    {
      ToolsTagsComboBox.DataContext = tagRepository.GetSortedShortTags(SitecoreToolsRegistryName, SitecoreToolsRegistryNamespace).ToArray();
      ToolsTagsComboBox.SelectedIndex = 0;
    }
    private void GetSpeAndSxaTags(string shortVersion, string topology)
    {
      if (int.Parse(shortVersion) > 101)
      {
        GetSpeTags(SitecoreSpeRegistryName);
        if (topology == "xm1")
        {
          GetSxaTags(SitecoreSxaRegistryNameXm1);
        }
        else
        {
          GetSxaTags(SitecoreSxaRegistryNameXp);
        }
      }
      else
      {
        GetSpeTags(SpeRegistryName);
        if (topology == "xm1")
        {
          GetSxaTags(SxaRegistryNameXm1);
        }
        else
        {
          GetSxaTags(SxaRegistryNameXp);
        }
      }
    }

    private void GetSpeTags(string speRegistryName)
    {
      SpeTagsComboBox.DataContext = tagRepository.GetSortedShortTags(speRegistryName, SitecoreModuleRegistryNamespace).ToArray();
      SpeTagsComboBox.SelectedIndex = 0;
    }

    private void GetSxaTags(string sxaRegistryName)
    {
      SxaTagsComboBox.DataContext = tagRepository.GetSortedShortTags(sxaRegistryName, SitecoreModuleRegistryNamespace).ToArray();
      SxaTagsComboBox.SelectedIndex = 0;
    }
  }
}