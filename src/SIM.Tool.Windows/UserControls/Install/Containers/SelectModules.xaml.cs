using SIM.ContainerInstaller;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SIM.Tool.Windows.Dialogs;
using SIM.Tool.Base;
using SIM.ContainerInstaller.Repositories.TagRepository;

namespace SIM.Tool.Windows.UserControls.Install.Containers
{
  public partial class SelectModules : IWizardStep, IFlowControl, ICustomButton
  {
    private Window owner;
    private EnvModel envModel;
    private ITagRepository tagRepository;
    private List<Module> selectedModules;

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
      tagRepository = args.TagRepository;
      HideTagsControls();
      
      int shortVersion = int.Parse(args.Product.ShortVersion);
      if (shortVersion < 100)
      {
        ModulesTextBlock.Text = "No modules are available.";
        ModulesListBox.Visibility = Visibility.Collapsed;
        return;
      }
      else
      {
        ModulesTextBlock.Text = "Modules:";
        ModulesListBox.Visibility = Visibility.Visible;
      }

      selectedModules = new List<Module>();
      List<Module> availableModules = new List<Module>() { Module.SXA, Module.JSS };
      GetSpeAndSxaTags(args.Product.ShortVersion, args.Topology);
      GetJssOrHeadlessTags(args.Product.ShortVersion, args.Topology);
      // Horizon and Publishing Service are available for Docker deployment only from 10.1.0
      if (shortVersion >= 101)
      {
        availableModules.Add(Module.Horizon);
        GetHorizonTags();
        GetHorizonAssetsTags(args.Topology);
        availableModules.Add(Module.PublishingService);
        GetSpsTags();
        GetSpsAssetsTags(args.Topology);
      }

      ModulesListBox.ItemsSource = availableModules;
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
        envModel.SitecoreModuleRegistry = $"{DockerSettings.SitecoreContainerRegistryHost}/{DockerSettings.SitecoreModuleNamespace}/";

        switch (module)
        {
          case Module.SXA:
            ShowOrHideSpeAndSxaTagsControls(Visibility.Visible);
            envModel.SpeVersion = SpeTagsComboBox.SelectedItem.ToString();
            envModel.SxaVersion = SxaTagsComboBox.SelectedItem.ToString();
            break;
          case Module.JSS:
            ShowOrHideJssTagsControls(Visibility.Visible);
            envModel.JssVersion = JssTagsComboBox.SelectedItem.ToString();
            break;
          case Module.Horizon:
            ShowOrHideHorizonTagsControls(Visibility.Visible);
            envModel.HorizonHost = string.Format(DockerSettings.HostNameTemplate, DockerSettings.HorizonServiceName, envModel.ProjectName);
            envModel.HorizonVersion = HorizonTagsComboBox.SelectedItem.ToString();
            envModel.HorizonAssetsVersion = HorizonAssetsTagsComboBox.SelectedItem.ToString();
            break;
          case Module.PublishingService:
            ShowOrHidePublishingServiceTagsControls(Visibility.Visible);
            envModel.SpsVersion = SpsTagsComboBox.SelectedItem.ToString();
            envModel.SpsAssetsVersion = SpsAssetsTagsComboBox.SelectedItem.ToString();
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
          envModel.Remove(EnvModel.SitecoreModuleRegistryName);
        }

        switch (module)
        {
          case Module.SXA:
            ShowOrHideSpeAndSxaTagsControls(Visibility.Collapsed);
            envModel.Remove(EnvModel.SpeVersionName);
            envModel.Remove(EnvModel.SxaVersionName);
            break;
          case Module.JSS:
            ShowOrHideJssTagsControls(Visibility.Collapsed);
            envModel.Remove(EnvModel.JssVersionName);
            break;
          case Module.Horizon:
            ShowOrHideHorizonTagsControls(Visibility.Collapsed);
            envModel.Remove(EnvModel.HorizonHostName);
            envModel.Remove(EnvModel.HorizonVersionName);
            envModel.Remove(EnvModel.HorizonAssetsVersionName);
            break;
          case Module.PublishingService:
            ShowOrHidePublishingServiceTagsControls(Visibility.Collapsed);
            envModel.Remove(EnvModel.SpsVersionName);
            envModel.Remove(EnvModel.SpsAssetsVersionName);
            break;
          default:
            break;
        }
      }
    }

    private void HideTagsControls()
    {
      SpeTagsTextBlock.Visibility = Visibility.Collapsed;
      SpeTagsComboBox.Visibility = Visibility.Collapsed;
      SxaTagsTextBlock.Visibility = Visibility.Collapsed;
      SxaTagsComboBox.Visibility = Visibility.Collapsed;
      JssTagsTextBlock.Visibility = Visibility.Collapsed;
      JssTagsComboBox.Visibility = Visibility.Collapsed;
      HorizonTagsTextBlock.Visibility = Visibility.Collapsed;
      HorizonTagsComboBox.Visibility = Visibility.Collapsed;
      HorizonAssetsTagsTextBlock.Visibility= Visibility.Collapsed;
      HorizonAssetsTagsComboBox.Visibility = Visibility.Collapsed;
      SpsTagsTextBlock.Visibility = Visibility.Collapsed;
      SpsTagsComboBox.Visibility = Visibility.Collapsed;
      SpsAssetsTagsTextBlock.Visibility = Visibility.Collapsed;
      SpsAssetsTagsComboBox.Visibility = Visibility.Collapsed;
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
      HorizonAssetsTagsTextBlock.Visibility = visibility;
      HorizonAssetsTagsComboBox.Visibility = visibility;
    }

    private void ShowOrHidePublishingServiceTagsControls(Visibility visibility)
    {
      SpsTagsTextBlock.Visibility = visibility;
      SpsTagsComboBox.Visibility = visibility;
      SpsAssetsTagsTextBlock.Visibility= visibility;
      SpsAssetsTagsComboBox.Visibility = visibility;
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

    private void GetSpeAndSxaTags(string shortVersion, string topology)
    {
      if (int.Parse(shortVersion) > 101)
      {
        GetSpeTags(DockerSettings.SitecoreSpeImagePath);
        if (topology == "xm1")
        {
          GetSxaTags(DockerSettings.SitecoreSxaXm1ImagePath);
        }
        else
        {
          GetSxaTags(DockerSettings.SitecoreSxaXpImagePath);
        }
      }
      else
      {
        GetSpeTags(DockerSettings.SpeImagePath);
        if (topology == "xm1")
        {
          GetSxaTags(DockerSettings.SxaXm1ImagePath);
        }
        else
        {
          GetSxaTags(DockerSettings.SxaXpImagePath);
        }
      }
    }

    private void GetSpeTags(string speImagePath)
    {
      SpeTagsComboBox.DataContext = tagRepository.GetSortedShortTags(speImagePath, DockerSettings.SitecoreModuleNamespace).ToArray();
      SpeTagsComboBox.SelectedIndex = 0;
    }

    private void GetSxaTags(string sxaImagePath)
    {
      SxaTagsComboBox.DataContext = tagRepository.GetSortedShortTags(sxaImagePath, DockerSettings.SitecoreModuleNamespace).ToArray();
      SxaTagsComboBox.SelectedIndex = 0;
    }

    private void JssTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (JssTagsComboBox.SelectedItem == null || !selectedModules.Contains(Module.JSS))
      {
        return;
      }

      envModel.JssVersion = JssTagsComboBox.SelectedItem.ToString();
    }

    private void GetJssOrHeadlessTags(string shortVersion, string topology)
    {
      if (int.Parse(shortVersion) >= 101)
      {
        if (topology == "xm1")
        {
          SetJssOrHeadlessTags(DockerSettings.SitecoreHeadlessServicesXm1ImagePath);
        }
        else
        {
          SetJssOrHeadlessTags(DockerSettings.SitecoreHeadlessServicesXpImagePath);
        }
      }
      else
      {
        if (topology == "xm1")
        {
          SetJssOrHeadlessTags(DockerSettings.JssXm1ImagePath);
        }
        else
        {
          SetJssOrHeadlessTags(DockerSettings.JssXpImagePath);
        }
      }
    }

    private void SetJssOrHeadlessTags(string jssOrHeadlessImagePath)
    {
      JssTagsComboBox.DataContext = tagRepository.GetSortedShortTags(jssOrHeadlessImagePath, DockerSettings.SitecoreModuleNamespace).ToArray();
      JssTagsComboBox.SelectedIndex = 0;
    }

    private void HorizonTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (HorizonTagsComboBox.SelectedItem == null || !selectedModules.Contains(Module.Horizon))
      {
        return;
      }

      envModel.HorizonVersion = HorizonTagsComboBox.SelectedItem.ToString();
    }

    private void HorizonAssetsTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (HorizonAssetsTagsComboBox.SelectedItem == null || !selectedModules.Contains(Module.Horizon))
      {
        return;
      }

      envModel.HorizonAssetsVersion = HorizonAssetsTagsComboBox.SelectedItem.ToString();
    }

    private void GetHorizonTags()
    {
      HorizonTagsComboBox.DataContext = tagRepository.GetSortedShortTags(DockerSettings.SitecoreHorizonImagePath, DockerSettings.SitecoreModuleNamespace).ToArray();
      HorizonTagsComboBox.SelectedIndex = 0;
    }

    private void GetHorizonAssetsTags(string topology)
    {
      string horizonAssetsImagePath = null;

      switch (topology)
      {
        case "xm1":
          horizonAssetsImagePath = DockerSettings.SitecoreHorizonAssetsXm1ImagePath;
          break;
        case "xp0":
          horizonAssetsImagePath = DockerSettings.SitecoreHorizonAssetsXp0ImagePath;
          break;
        case "xp1":
          horizonAssetsImagePath = DockerSettings.SitecoreHorizonAssetsXp1ImagePath;
          break;
        default:
          break;
      }

      if (!string.IsNullOrEmpty(horizonAssetsImagePath))
      {
        HorizonAssetsTagsComboBox.DataContext = tagRepository.GetSortedShortTags(horizonAssetsImagePath, DockerSettings.SitecoreModuleNamespace).ToArray();
        HorizonAssetsTagsComboBox.SelectedIndex = 0;
      }
    }

    private void SpsTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (SpsTagsComboBox.SelectedItem == null || !selectedModules.Contains(Module.PublishingService))
      {
        return;
      }

      envModel.SpsVersion = SpsTagsComboBox.SelectedItem.ToString();
    }

    private void SpsAssetsTagsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (SpsAssetsTagsComboBox.SelectedItem == null || !selectedModules.Contains(Module.PublishingService))
      {
        return;
      }

      envModel.SpsAssetsVersion = SpsAssetsTagsComboBox.SelectedItem.ToString();
    }

    private void GetSpsTags()
    {
      SpsTagsComboBox.DataContext = tagRepository.GetSortedShortTags(DockerSettings.SpsImagePath, DockerSettings.SitecoreModuleNamespace).ToArray();
      SpsTagsComboBox.SelectedIndex = 0;
    }

    private void GetSpsAssetsTags(string topology)
    {
      string spsAssetsImagePath = null;

      switch (topology)
      {
        case "xm1":
          spsAssetsImagePath = DockerSettings.SpsAssetsXm1ImagePath;
          break;
        case "xp0":
          spsAssetsImagePath = DockerSettings.SpsAssetsXp0ImagePath;
          break;
        case "xp1":
          spsAssetsImagePath = DockerSettings.SpsAssetsXp1ImagePath;
          break;
        default:
          break;
      }

      if (!string.IsNullOrEmpty(spsAssetsImagePath))
      {
        SpsAssetsTagsComboBox.DataContext = tagRepository.GetSortedShortTags(spsAssetsImagePath, DockerSettings.SitecoreModuleNamespace).ToArray();
        SpsAssetsTagsComboBox.SelectedIndex = 0;
      }
    }
  }
}