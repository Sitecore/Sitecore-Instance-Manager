namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  [UsedImplicitly]
  public class ConfigBuilderButton : IMainWindowButton
  {
    #region Constants

    private const string BaseUrl = "http://dl.sitecore.net/updater/1.1/scb/";

    #endregion

    #region Fields

    protected readonly bool Normalize;
    protected readonly bool Showconfig;
    protected readonly bool WebConfigResult;

    #endregion

    #region Constructors

    public ConfigBuilderButton()
    {
      this.Normalize = false;
      this.WebConfigResult = false;
      this.Showconfig = false;
    }

    public ConfigBuilderButton(string param)
    {
      this.Normalize = param.ContainsIgnoreCase("/normalize");
      this.Showconfig = param.ContainsIgnoreCase("/showconfig");
      this.WebConfigResult = param.ContainsIgnoreCase("/webconfigresult");
      Assert.IsTrue(!(this.Showconfig && this.WebConfigResult), "/showconfig and /webconfigresult params must not be used together");
    }

    #endregion

    #region Public methods

    public static void RunConfigApp(string name, Window mainWindow, string param = null)
    {
      string path = Path.Combine(ApplicationManager.TempFolder, "Config Builder\\" + name);

      var latestVersion = GetLatestVersion();

      if (!FileSystem.FileSystem.Local.File.Exists(path) || (!string.IsNullOrEmpty(latestVersion) && FileVersionInfo.GetVersionInfo(path).ProductVersion != latestVersion))
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

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (!this.Showconfig && !this.WebConfigResult)
      {
        RunConfigApp("Sitecore.ConfigBuilder.Tool.exe", mainWindow, instance != null ? Path.Combine(instance.WebRootPath, "web.config") : null);
        return;
      }

      var folder = Path.Combine(ApplicationManager.TempFolder, "configs", instance.Name);
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }

      string path;
      if (this.Showconfig)
      {
        path = Path.Combine(folder, "showconfig.xml");
      }
      else if (this.WebConfigResult)
      {
        path = Path.Combine(folder, "web.config.result.xml");
      }
      else
      {
        throw new NotSupportedException("This is not supported");
      }

      if (this.Normalize)
      {
        path += ".normalized.xml";
      }

      if (this.Showconfig)
      {
        instance.GetShowconfig(this.Normalize).Save(path);
      }
      else if (this.WebConfigResult)
      {
        instance.GetWebResultConfig(this.Normalize).Save(path);
      }
      else
      {
        Assert.IsTrue(false, "Impossible");
      }

      WindowHelper.OpenFile(path);
    }

    #endregion

    #region Private methods

    private static string GetLatestVersion()
    {
      var latestVersion = string.Empty;
      var url = BaseUrl + "latest-version.txt";
      try
      {
        latestVersion = WebRequestHelper.DownloadString(url).Trim();
      }
      catch (Exception ex)
      {
        Log.Warn("The {0} URL is unavailable".FormatWith(url), typeof(ConfigBuilderButton), ex);
      }

      return latestVersion;
    }

    private static void GetLatestVersion(Window mainWindow, string path)
    {
      WindowHelper.LongRunningTask(() => GetLatestVersion(path), "Downloading latest version", mainWindow, 
        "Downloading latest version of Sitecore ConfigBuilder. \n\nNext time this operation will not be needed.", 
        "It may take a few minutes if you have slow internet connection", true);
    }

    private static void GetLatestVersion(string path)
    {
      try
      {
        var folder = Path.GetDirectoryName(path);
        FileSystem.FileSystem.Local.Directory.Ensure(folder);
        var downloadUrl = WebRequestHelper.DownloadString(BaseUrl + "download.txt");
        var packageZipPath = Path.Combine(folder, "package.zip");
        WebRequestHelper.DownloadFile(downloadUrl, packageZipPath);
        FileSystem.FileSystem.Local.Zip.UnpackZip(packageZipPath, folder);
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Couldn't get latest version of Sitecore ConfigBuilder", true, ex, typeof(ConfigBuilderButton));
      }
    }

    #endregion
  }
}