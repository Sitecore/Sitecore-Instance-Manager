namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using Microsoft.Win32;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Wizards;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class ImportInstanceButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      var fileDialog = new OpenFileDialog
      {
        Title = "Select zip file of exported solution", 
        Multiselect = false, 
        DefaultExt = ".zip"
      };

      fileDialog.ShowDialog();
      string filePath = fileDialog.FileName;
      if (!string.IsNullOrEmpty(filePath))
      {
        if (FileSystem.FileSystem.Local.Zip.ZipContainsFile(filePath, "AppPoolSettings.xml") && FileSystem.FileSystem.Local.Zip.ZipContainsFile(filePath, "WebsiteSettings.xml") && FileSystem.FileSystem.Local.Zip.ZipContainsFile(filePath, @"Website/Web.config"))
        {
          WizardPipelineManager.Start("import", mainWindow, null, null, MainWindowHelper.SoftlyRefreshInstances, filePath);
        }
        else
        {
          WindowHelper.ShowMessage("Wrong package for import.");
        }
      }
    }

    #endregion
  }
}