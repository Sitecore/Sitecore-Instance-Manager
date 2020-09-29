namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Core;

  [UsedImplicitly]
  public class InstallMongoDbButton : WindowOnlyButton
  {
    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      CoreApp.RunApp(ApplicationManager.GetEmbeddedFile(@"MongoDb.WindowsService.Installer.zip", "SIM.Tool.Windows", @"MongoDb.WindowsService.Installer.exe"));
    }

    #endregion
  }
}