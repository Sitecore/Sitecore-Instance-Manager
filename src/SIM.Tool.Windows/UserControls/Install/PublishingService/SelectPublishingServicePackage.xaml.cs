using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Navigation;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.UserControls.Install.PublishingService
{
  /// <summary>
  /// Interaction logic for SelectPublishingServicePackage.xaml
  /// </summary>
  public partial class SelectPublishingServicePackage : IWizardStep, IFlowControl
  {
    private const string KB_SPS_COMPATIBILTY_PRIOR_TO_410 = @"https://kb.sitecore.net/articles/545289";
    private const string KB_SPS_COMPATIBILTY_410_AND_LATER = @"https://kb.sitecore.net/articles/761308";
    private const string KB_SXA_COMPATIBILITY_WITH_SPS = @"https://kb.sitecore.net/articles/180187#CompatibilityWithSitecorePublishingService";

    public SelectPublishingServicePackage()
    {
      InitializeComponent();
      Initialize();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      return;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      InstallSPSWizardArgs args = (InstallSPSWizardArgs)wizardArgs;
      args.SPSPackage = ((ComboBoxItem)SPSPackageComboBox.SelectedItem)?.Tag.ToString();

      if (args.SPSPackage == null)
      {
        WindowHelper.ShowMessage("Please select a package before continuing!");
        return false;
      }

      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    private void Initialize()
    {
      //Prepare links
      DownloadSPSLink.NavigateUri = new Uri(@"https://dev.sitecore.net/Downloads/Sitecore_Publishing_Service.aspx");
      OpenLocalRepoLink.NavigateUri = new Uri($@"{ProfileManager.Profile.LocalRepository}");
      KBForSPSCompatibilityPriorTo410.NavigateUri = new Uri(KB_SPS_COMPATIBILTY_PRIOR_TO_410);
      KBForSPSCompatibility410AndLater.NavigateUri = new Uri(KB_SPS_COMPATIBILTY_410_AND_LATER);
      KBForSXACompatibilityPriorWithSPS.NavigateUri = new Uri(KB_SXA_COMPATIBILITY_WITH_SPS);

      //Populate ComboBox
      foreach (string packagePath in Directory.GetFiles(ProfileManager.Profile.LocalRepository, "Sitecore Publishing Service*.zip"))
      {
        SPSPackageComboBox.Items.Add(new ComboBoxItem()
        {
         Content = Path.GetFileNameWithoutExtension(packagePath),
         Tag = packagePath
        });
      }
      return;
    }
  }
}
