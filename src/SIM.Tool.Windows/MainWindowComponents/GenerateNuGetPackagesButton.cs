namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;
  using SIM.Core;
  using SIM.Tool.Base;

  [UsedImplicitly]
  public class GenerateNuGetPackagesButton : IMainWindowButton
  {
    public GenerateNuGetPackagesButton()
    {
    }

    public GenerateNuGetPackagesButton(string mode)
    {
    }
    
    public bool IsEnabled([CanBeNull] Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      WindowHelper.ShowMessage("This function is no longer available. Use PatchCreator to generate NuGet packages for Sitecore CMS and Sitecore Modules.");

      CoreApp.OpenInBrowser("http://dl.sitecore.net/updater/pc", true);
    }
  }
}