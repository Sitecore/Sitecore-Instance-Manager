namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using Microsoft.Win32;
  using SIM.Core.Common;
  using SIM.FileSystem;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Tool.Base.Wizards;

  [UsedImplicitly]
  public class ImportInstanceButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled([CanBeNull] Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      Analytics.TrackEvent("Import");

      var fileDialog = new OpenFileDialog
      {
        Title = "Select zip file of exported solution",
        Multiselect = false,
        DefaultExt = ".zip"
      };

      fileDialog.ShowDialog();
      var filePath = fileDialog.FileName;
      if (string.IsNullOrEmpty(filePath))
      {
        return;
      }

      const string AppPoolFileName = "AppPoolSettings.xml";
      var appPool = FileSystem.Local.Zip.ZipContainsFile(filePath, AppPoolFileName);
      if (!appPool)
      {
        WindowHelper.ShowMessage("Wrong package for import. The package does not contain the {0} file.".FormatWith(AppPoolFileName));
        return;
      }

      const string WebsiteSettingsFileName = "WebsiteSettings.xml";
      var websiteSettings = FileSystem.Local.Zip.ZipContainsFile(filePath, WebsiteSettingsFileName);
      if (!websiteSettings)
      {
        WindowHelper.ShowMessage("Wrong package for import. The package does not contain the {0} file.".FormatWith(WebsiteSettingsFileName));

        return;
      }

      const string WebConfigFileName = @"Website/Web.config";
      if (!FileSystem.Local.Zip.ZipContainsFile(filePath, WebConfigFileName))
      {
        WindowHelper.ShowMessage("Wrong package for import. The package does not contain the {0} file.".FormatWith(WebConfigFileName));

        return;
      }

      WizardPipelineManager.Start("import", mainWindow, null, null, MainWindowHelper.SoftlyRefreshInstances, filePath);
    }

    #endregion
  }
}