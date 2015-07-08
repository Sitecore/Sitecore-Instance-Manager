using System.IO;
using System.Text.RegularExpressions;
using SIM.Base;
using SIM.Tool.Base.Wizards;
using System.Windows;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.UserControls.Export
{
  using System;

  public partial class ExportFile : IWizardStep
  {
    public ExportFile()
    {
      InitializeComponent();
    }

    private void PickExportFileClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      var dialog = new Microsoft.Win32.SaveFileDialog { DefaultExt = ".zip", Filter = "Zip archive(.zip)|*.zip" };
      var result = dialog.ShowDialog();
      if (result == true) ExportedFile.Text = dialog.FileName;
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      ExportedFile.Text = ((ExportWizardArgs)wizardArgs).ExportFilePath;
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (ExportWizardArgs)wizardArgs;
      args.ExportFilePath = ExportedFile.Text;
      args.IncludeTempFolderContents = !(ExcludeTempFolderContents.IsChecked ?? true);
      args.IncludeMediaCacheFolderContents = !(ExcludeMediaCacheFolderContents.IsChecked ?? true);
      args.ExcludeLicenseFile = ExcludeLicenseFile.IsChecked ?? false;
      args.ExcludeDiagnosticsFolderContents = ExcludeDiagnosticsFolderContents.IsChecked ?? false;
      args.ExcludeLogsFolderContents = ExcludeLogsFolderContents.IsChecked ?? false;
      args.ExcludePackagesFolderContents = ExcludePackagesFolderContents.IsChecked ?? false;
      args.ExcludeUploadFolderContents = ExcludeUploadFolderContents.IsChecked ?? false;

      if (IsPathValid())
      {
        var path = Path.GetDirectoryName(ExportedFile.Text);

        if (!string.IsNullOrEmpty(path))
        {
          FileSystem.Local.Directory.CreateDirectory(path);
          return true;
        }
      }

      WindowHelper.ShowMessage("You have specified the incorrect path or file name.");
      return false;
    }

    private bool IsPathValid()
    {
      var filePath = ExportedFile.Text;
      try
      {
        ExportedFile.Text = Path.GetFullPath(filePath);
        return Path.IsPathRooted(filePath);
      }
      catch(Exception ex)
      {
        Log.Warn("An error occurred during checking if the path is valid", this, ex);
        return false;
      }
    }
  }
}
