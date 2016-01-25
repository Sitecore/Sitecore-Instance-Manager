using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Sitecore.Diagnostics.Logging;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public abstract class AbstractDownloadAndRunButton
  {
    protected abstract string BaseUrl { get; }

    protected abstract string AppName { get; }

    protected abstract string ExecutableName { get; }

    protected void RunApp(Window mainWindow, string param = null)
    {
      string path = Path.Combine(ApplicationManager.TempFolder, this.AppName + "\\" + this.ExecutableName);

      var latestVersion = GetLatestVersion();

      if (!FileSystem.FileSystem.Local.File.Exists(path) || (!String.IsNullOrEmpty(latestVersion) && FileVersionInfo.GetVersionInfo(path).ProductVersion != latestVersion))
      {
        GetLatestVersion(mainWindow, path);

        if (!FileSystem.FileSystem.Local.File.Exists(path))
        {
          return;
        }
      }

      if (param != null)
      {
        WindowHelper.RunApp(path, param);
        return;
      }

      WindowHelper.RunApp(path);
    }

    #region Private methods

    private string GetLatestVersion()
    {
      var latestVersion = String.Empty;
      var url = this.BaseUrl.TrimEnd('/') + "/latest-version.txt";
      try
      {
        latestVersion = WebRequestHelper.DownloadString(url).Trim();
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "The {0} URL is unavailable", url);
      }

      return latestVersion;
    }

    private void GetLatestVersion(Window mainWindow, string path)
    {
      WindowHelper.LongRunningTask(() => this.GetLatestVersion(path), "Downloading latest version", mainWindow,
        "Downloading latest version of " + this.AppName + ". \n\nNext time this operation will not be needed.",
        "It may take a few minutes if you have slow internet connection", true);
    }

    private void GetLatestVersion(string path)
    {
      try
      {
        var folder = Path.GetDirectoryName(path);
        FileSystem.FileSystem.Local.Directory.Ensure(folder);
        var downloadUrl = WebRequestHelper.DownloadString(this.BaseUrl.TrimEnd('/') + "/download.txt");
        var packageZipPath = Path.Combine(folder, "package.zip");
        WebRequestHelper.DownloadFile(downloadUrl, packageZipPath);
        FileSystem.FileSystem.Local.Zip.UnpackZip(packageZipPath, folder);
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Couldn't get latest version of Sitecore ConfigBuilder", true, ex);
      }
    }

    #endregion
  }
}