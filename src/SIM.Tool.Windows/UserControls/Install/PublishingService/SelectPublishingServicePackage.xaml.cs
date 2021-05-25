using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Navigation;
using SIM.Tool.Base;
using SIM.Tool.Base.Configuration;
using Log = Sitecore.Diagnostics.Logging.Log;

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

    private static readonly Dictionary<string, int[]> CompatibilityTable = Configuration.Instance.SitecorePublishingServiceCompatibility;

    public SelectPublishingServicePackage()
    {
      InitializeComponent();
      Initialize();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      InstallSPSWizardArgs args = (InstallSPSWizardArgs)wizardArgs;
      int cmsVersionInt = args.CMInstance.Product?.Release?.Version.MajorMinorUpdateInt ?? -1;

      //Populate ComboBox
      foreach (string packagePath in Directory.GetFiles(ProfileManager.Profile.LocalRepository, "Sitecore Publishing Service*.zip", SearchOption.AllDirectories))
      {
        string spsPackageName = Path.GetFileNameWithoutExtension(packagePath);
        if (IsCompatible(spsPackageName, cmsVersionInt))
        {
          SPSPackageComboBox.Items.Add(new ComboBoxItem()
          {
            Content = spsPackageName,
            Tag = packagePath
          });
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
      args.SPSPackage = ((ComboBoxItem)SPSPackageComboBox.SelectedItem)?.Tag.ToString();

      if (args.SPSPackage == null)
      {
        WindowHelper.ShowMessage("Please select a package before continuing!");
        return false;
      }

      args.SPSVersionInt = int.Parse(GetSPSVersion(args.SPSPackage));

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

      return;
    }

    private string GetSPSVersion(string spsPackageName)
    {
      string spsPackageVersion = "-1";

      try
      {

        //Check for special cases in naming convention
        if (spsPackageName.StartsWith("Sitecore Publishing Service 312"))
        {
          spsPackageVersion = "312";
        }
        else
        {
          Regex versionPattern = new Regex("\\d+\\.\\d+\\.\\d+");
          spsPackageVersion = versionPattern.Match(spsPackageName).Value.Replace(".", "");
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, $"The version of the package '{spsPackageName}' could not be resolved");
      }

      return spsPackageVersion;
    }

    private bool IsCompatible(string spsPackageName, int cmsVersionInt)
    {
      try
      {
        string spsPackageVersion = GetSPSVersion(spsPackageName);

        //Get the array of compatible cms versions for the given sps version, then check if the selected instance's version is in this array
        return (from pair in CompatibilityTable 
          where spsPackageVersion == pair.Key 
          select pair.Value.Any(i => i == cmsVersionInt)).FirstOrDefault();   
      }
      catch (Exception ex)
      {
        string message = "Something went wrong while trying to resolve compatible SPS versions.\nAs a result all options are shown.  Consult the compatibility tables to ensure your SPS and Sitecore version are compatible.";

        WindowHelper.ShowMessage(message);
        Log.Error($"{message}\n{ex}");
        
        return true;  //If we can't determine compatibility, let's not restrict anything
      }
      
    }
  }
}
