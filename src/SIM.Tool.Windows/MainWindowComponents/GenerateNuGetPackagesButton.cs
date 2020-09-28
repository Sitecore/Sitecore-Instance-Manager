namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using JetBrains.Annotations;
  using SIM.Core;
  using SIM.Tool.Base;

  [UsedImplicitly]
  public class GenerateNuGetPackagesButton : WindowOnlyButton
  {
    public GenerateNuGetPackagesButton()
    {
    }

    public GenerateNuGetPackagesButton(string mode)
    {
    }

    protected override void OnClick(Window mainWindow)
    {
      WindowHelper.ShowMessage("This function is no longer available. Use PatchCreator to generate NuGet packages for Sitecore CMS and Sitecore Modules.");

      CoreApp.OpenInBrowser("http://dl.sitecore.net/updater/pc", true);
    }
  }
}