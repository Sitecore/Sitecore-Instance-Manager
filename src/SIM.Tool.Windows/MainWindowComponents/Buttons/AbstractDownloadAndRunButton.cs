using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using JetBrains.Annotations;
using SIM.Core;
using SIM.Instances;
using SIM.Tool.Base;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Logging;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public abstract class AbstractDownloadAndRunButton : WindowOnlyButton
  {
    [NotNull]
    protected abstract string BaseUrl { get; }

    [NotNull]
    protected abstract string AppName { get; }

    [NotNull]
    protected abstract string ExecutableName { get; }

    [NotNull]
    protected string AppFolder => Path.Combine(ApplicationManager.AppsFolder, AppName);

    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
    {
      RunApp(mainWindow);
    }

    #endregion

    protected void RunApp(Window mainWindow, string param = null)
    {
      var path = Path.Combine(AppFolder, ExecutableName);

      var latestVersion = GetLatestVersion();

      if (!FileSystem.FileSystem.Local.File.Exists(path) || (!string.IsNullOrEmpty(latestVersion) && FileVersionInfo.GetVersionInfo(path).ProductVersion != latestVersion))
      {
        this.GetLatestVersion(mainWindow, path);

        if (!FileSystem.FileSystem.Local.File.Exists(path))
        {
          return;
        }
      }

      RunApp(path, param);
    }

    protected virtual void RunApp([NotNull] string path, [CanBeNull] string param)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));

      if (param != null)
      {
        CoreApp.RunApp(path, param);
      }
      else
      {
        CoreApp.RunApp(path);
      }
    }

    #region Private methods

    private string GetLatestVersion()
    {
      var latestVersion = string.Empty;
      var url = BaseUrl.TrimEnd('/') + "/latest-version.txt";
      try
      {
        latestVersion = WebRequestHelper.DownloadString(url).Trim();
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"The {url} URL is unavailable");
      }

      return latestVersion;
    }

    private void GetLatestVersion(Window mainWindow, string path)
    {
      WindowHelper.LongRunningTask(() => GetLatestVersion(path), "Downloading latest version", mainWindow,
        "Downloading latest version of " + AppName + ". \n\nNext time this operation will not be needed.",
        "It may take a few minutes if you have slow internet connection", true);
    }

    private void GetLatestVersion(string path)
    {
      try
      {
        var folder = Path.GetDirectoryName(path);
        FileSystem.FileSystem.Local.Directory.Ensure(folder);
        var downloadTxtUrl = BaseUrl.TrimEnd('/') + "/download.txt";
        var downloadUrl = WebRequestHelper.DownloadString(downloadTxtUrl).TrimEnd(" \r\n".ToCharArray());
        var packageZipPath = Path.Combine(folder, "package.zip");
        WebRequestHelper.DownloadFile(downloadUrl, packageZipPath);
        FileSystem.FileSystem.Local.Zip.UnpackZip(packageZipPath, folder);
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Couldn't get latest version of " + AppName, true, ex);
      }
    }

    #endregion
  }
}