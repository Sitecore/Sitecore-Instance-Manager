using System.Windows;
using Microsoft.Win32;
using SIM.Instances;
using SIM.Tool.Base.Plugins;
using SIM.Base;
using SIM.Tool.Base;
using SIM.Tool.Wizards;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class ImportInstanceButton : IMainWindowButton
  {
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
        if (FileSystem.Local.Zip.ZipContainsFile(filePath, "AppPoolSettings.xml") && FileSystem.Local.Zip.ZipContainsFile(filePath, "WebsiteSettings.xml") && FileSystem.Local.Zip.ZipContainsFile(filePath, @"Website/Web.config"))
          {
            WizardPipelineManager.Start("import", mainWindow, null, null, MainWindowHelper.SoftlyRefreshInstances, filePath);
          }
          else
          {
              WindowHelper.ShowMessage("Wrong package for import.");
          }
      }
    }
  }
}
