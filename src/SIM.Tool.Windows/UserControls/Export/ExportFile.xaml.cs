namespace SIM.Tool.Windows.UserControls.Export
{
  using System;
  using System.IO;
  using System.Windows;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public partial class ExportFile : IWizardStep
  {
    #region Constructors

    public ExportFile()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      this.ExportedFile.Text = ((ExportWizardArgs)wizardArgs).ExportFilePath;
    }

    private bool IsPathValid()
    {
      var filePath = this.ExportedFile.Text;
      try
      {
        this.ExportedFile.Text = Path.GetFullPath(filePath);
        return Path.IsPathRooted(filePath);
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "An error occurred during checking if the path is valid");
        return false;
      }
    }

    private void PickExportFileClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      var dialog = new Microsoft.Win32.SaveFileDialog
      {
        DefaultExt = ".zip", 
        Filter = "Zip archive(.zip)|*.zip"
      };
      var result = dialog.ShowDialog();
      if (result == true)
      {
        this.ExportedFile.Text = dialog.FileName;
      }
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (ExportWizardArgs)wizardArgs;
      args.ExportFilePath = this.ExportedFile.Text;
      args.IncludeTempFolderContents = !(this.ExcludeTempFolderContents.IsChecked ?? true);
      args.IncludeMediaCacheFolderContents = !(this.ExcludeMediaCacheFolderContents.IsChecked ?? true);
      args.ExcludeLicenseFile = this.ExcludeLicenseFile.IsChecked ?? false;
      args.ExcludeDiagnosticsFolderContents = this.ExcludeDiagnosticsFolderContents.IsChecked ?? false;
      args.ExcludeLogsFolderContents = this.ExcludeLogsFolderContents.IsChecked ?? false;
      args.ExcludePackagesFolderContents = this.ExcludePackagesFolderContents.IsChecked ?? false;
      args.ExcludeUploadFolderContents = this.ExcludeUploadFolderContents.IsChecked ?? false;

      if (this.IsPathValid())
      {
        var path = Path.GetDirectoryName(this.ExportedFile.Text);

        if (!string.IsNullOrEmpty(path))
        {
          FileSystem.FileSystem.Local.Directory.CreateDirectory(path);
          return true;
        }
      }

      WindowHelper.ShowMessage("You have specified the incorrect path or file name.");
      return false;
    }

    #endregion
  }
}