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
using Microsoft.VisualBasic.Logging;
using SIM.Tool.Base;
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

    public SelectPublishingServicePackage()
    {
      InitializeComponent();
      Initialize();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      InstallSPSWizardArgs args = (InstallSPSWizardArgs)wizardArgs;
      int cmsVersionInt = args.Instance.Product?.Release?.Version.MajorMinorUpdateInt ?? -1;

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

    private bool IsCompatible(string spsPackageName, int cmsVersionInt)
    {
      try
      {
        int spsPackageVersionInt = -1;

        //Check for special cases in naming convention
        if (spsPackageName.StartsWith("Sitecore Publishing Service 312"))
        {
          spsPackageVersionInt = 312;
        }
        else
        {
          Regex versionPattern = new Regex("\\d+\\.\\d+\\.\\d+");
          spsPackageVersionInt = int.Parse(versionPattern.Match(spsPackageName).Value.Replace(".", ""));
        }

        //sps version 410 and above
        if (spsPackageVersionInt >= 410)
        {
          /*
           * If cmsVersionInt is negative, it means there was an error getting the version, and we should not restrict options.
           * This can happen if a new version of Sitecore is released and SIM cannot parse its version number yet.
           * Since we are assuming this is a new version of Sitecore, it is still safe to allow compatibility for SPS version 4.1.0 and above
           */
          return cmsVersionInt >= 910 || cmsVersionInt < 0;
        }

        //sps version 400 and below
        return (from pair in LegacySPSCompatibilityTable
            where spsPackageVersionInt == pair.Key //Get the array of compatible cms versions for the given sps version
            select pair.Value.Any(i => i == cmsVersionInt))
          .FirstOrDefault(); //Check if the selected instance's version is in this array
      }
      catch (Exception ex)
      {
        string message = $"Something went wrong while trying to resolve compatible SPS versions.\nThe package '{spsPackageName}' may not be valid.\n" +
          $"As a result all options are shown.  Consult the compatibility tables to ensure your SPS and Sitecore version are compatible.";

        WindowHelper.ShowMessage(message);
        Log.Error($"{message}\n{ex}");
        
        return true;  //If we can't determine compatibility, let's not restrict anything
      }
      
    }

    private static Dictionary<int, int[]> LegacySPSCompatibilityTable
    {
      get
      {
        return new Dictionary<int, int[]>()
        {
          { 111, new [] { 820, 821 }},
          { 200, new [] { 822, 823 }},
          { 201, new [] { 822, 823, 824, 825 }},
          { 210, new [] { 822, 823, 824, 825, 826 }},
          { 220, new [] { 822, 823, 824, 825, 826, 827 }},
          { 221, new [] { 822, 823, 824, 825, 826, 827 }},
          { 300, new [] { 900 }},
          { 310, new [] { 900, 901, 902 }},
          { 311, new [] { 900, 901, 902 }},
          { 312, new [] { 900, 901, 902 }},
          { 313, new [] { 900, 901, 902 }},
          { 314, new [] { 900, 901, 902 }},
          { 400, new [] { 910, 911 }}
        };
      }
    }
  }
}
