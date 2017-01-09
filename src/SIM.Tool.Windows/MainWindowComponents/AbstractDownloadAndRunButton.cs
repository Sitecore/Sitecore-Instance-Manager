using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Sitecore.Diagnostics.Logging;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.MainWindowComponents
{
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Core;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;

  public abstract class AbstractDownloadAndRunButton : IMainWindowButton
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

    public virtual bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public virtual void OnClick(Window mainWindow, Instance instance)
    {
      Analytics.TrackEvent("OpenCommandLine");             

      RunApp(mainWindow);
    }

    #endregion

    protected void RunApp(Window mainWindow, string param = null)
    {
      var path = Path.Combine(AppFolder, this.ExecutableName);

      var latestVersion = GetLatestVersion();

      if (!FileSystem.FileSystem.Local.File.Exists(path) || (!string.IsNullOrEmpty(latestVersion) && FileVersionInfo.GetVersionInfo(path).ProductVersion != latestVersion))
      {
        GetLatestVersion(mainWindow, path);

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
      var url = this.BaseUrl.TrimEnd('/') + "/latest-version.txt";
      try
      {
        latestVersion = WebRequestHelper.DownloadString(url).Trim();
      }
      catch (Exception ex)
      {
        Log.Warn(ex, string.Format("The {0} URL is unavailable", url));
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
        var downloadTxtUrl = this.BaseUrl.TrimEnd('/') + "/download.txt";
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