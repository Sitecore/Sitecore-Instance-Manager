namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using Microsoft.Win32;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;
  using SIM.IO.Real;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.UserControls.Import;

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
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

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

      Log.Info($"Importing solution from {filePath}");
      var fileSystem = new RealFileSystem();
      var file = fileSystem.ParseFile(filePath);
      using (var zipFile = new RealZipFile(fileSystem.ParseFile(file.FullName)))
      {
        const string AppPoolFileName = "AppPoolSettings.xml";
        var appPool = zipFile.Entries.Contains(AppPoolFileName);
        if (!appPool)
        {
          WindowHelper.ShowMessage("Wrong package for import. The package does not contain the {0} file.".FormatWith(AppPoolFileName));
          return;
        }

        const string WebsiteSettingsFileName = "WebsiteSettings.xml";
        var websiteSettings = zipFile.Entries.Contains(WebsiteSettingsFileName);
        if (!websiteSettings)
        {
          WindowHelper.ShowMessage("Wrong package for import. The package does not contain the {0} file.".FormatWith(WebsiteSettingsFileName));

          return;
        }

        const string WebConfigFileName = @"Website/Web.config";
        if (!zipFile.Entries.Contains(WebConfigFileName))
        {
          WindowHelper.ShowMessage("Wrong package for import. The package does not contain the {0} file.".FormatWith(WebConfigFileName));

          return;
        }
      }

      WizardPipelineManager.Start("import", mainWindow, null, null, ignore => MainWindowHelper.SoftlyRefreshInstances(), () => new ImportWizardArgs(file.FullName));
    }

    #endregion
  }
}