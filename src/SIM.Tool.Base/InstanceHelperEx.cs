namespace SIM.Tool.Base
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Windows;
  using Microsoft.Web.Administration;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public static class InstanceHelperEx
  {
    #region Public methods

    public static void BrowseInstance([NotNull] Instance instance, [NotNull] Window owner, [NotNull] string virtualPath, bool isFrontEnd, [CanBeNull] string browser = null, [CanBeNull] string[] parameters = null)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(owner, "owner");
      Assert.ArgumentNotNull(virtualPath, "virtualPath");

      if (!EnsureAppPoolState(instance, owner))
      {
        return;
      }

      string url = instance.GetUrl();
      if (!string.IsNullOrEmpty(url))
      {
        url += '/' + virtualPath.TrimStart('/');
        WindowHelper.OpenInBrowser(url, isFrontEnd, browser, parameters);
      }
    }

    public static void OpenCurrentLogFile([NotNull] Instance instance, [NotNull] Window owner, [CanBeNull] string logFileType = null)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(owner, "owner");

      string dataFolderPath = instance.DataFolderPath;
      FileSystem.FileSystem.Local.Directory.AssertExists(dataFolderPath, "The data folder ({0}) of the {1} instance doesn't exist".FormatWith(dataFolderPath, instance.Name));
      var logs = instance.LogsFolderPath;
      logFileType = logFileType ?? GetLogFileTypes(owner, logs);
      if (logFileType == null)
      {
        return;
      }

      var pattern = logFileType + "*.txt";
      var files = FileSystem.FileSystem.Local.Directory.GetFiles(logs, pattern) ?? new string[0];
      var logFilePath = files.OrderByDescending(FileSystem.FileSystem.Local.File.GetCreationTimeUtc).FirstOrDefault();
      if (string.IsNullOrEmpty(logFilePath))
      {
        return;
      }

      string logviewer = WinAppSettings.AppToolsLogViewer.Value;
      if (string.IsNullOrEmpty(logviewer))
      {
        return;
      }

      if (logviewer == "logview.exe")
      {
        logviewer = ApplicationManager.GetEmbeddedFile("logview.zip", "SIM.Tool.Windows", "logview.exe");
      }

      var fileSystemWatcher = new FileSystemWatcher(logs)
      {
        Filter = pattern, 
        IncludeSubdirectories = false
      };

      bool ignore = false;

      var process = WindowHelper.RunApp(logviewer, logFilePath);
      if (process == null)
      {
        return;
      }

      // we need to stop all this magic when application closes
      process.Exited += (sender, args) =>
      {
        // but shouldn't if it is initiated by this magic
        if (ignore)
        {
          ignore = false;
          return;
        }

        fileSystemWatcher.EnableRaisingEvents = false;
      };

      fileSystemWatcher.Created += (sender, args) =>
      {
        try
        {
          // indicate that magic begins
          ignore = true;

          // magic begins
          process.Kill();
          files = FileSystem.FileSystem.Local.Directory.GetFiles(logs, pattern) ?? new string[0];
          logFilePath = files.OrderByDescending(FileSystem.FileSystem.Local.File.GetCreationTimeUtc).First();
          process = WindowHelper.RunApp(logviewer, logFilePath);
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Unhandled error happened while reopening log file");
        }
        finally
        {
          fileSystemWatcher.EnableRaisingEvents = false;
        }
      };

      fileSystemWatcher.EnableRaisingEvents = true;
    }

    public static void OpenInBrowserAsAdmin([NotNull] Instance instance, [NotNull] Window owner, [CanBeNull] string pageUrl = null, [CanBeNull] string browser = null, [CanBeNull] string[] parameters = null)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(owner, "owner");

      AuthenticationHelper.LoginAsAdmin(instance, owner, pageUrl, browser, parameters);
    }

    public static bool PreheatInstance(Instance instance, Window mainWindow, bool ignoreAdvancedSetting = false)
    {
      if (!EnsureAppPoolState(instance, mainWindow))
      {
        return false;
      }

      if (!WinAppSettings.AppPreheatEnabled.Value && !ignoreAdvancedSetting)
      {
        return true;
      }

      // Check if the instance is responsive now
      if (!InstanceHelper.IsInstanceResponsive(instance, "fast"))
      {
        // It is not responsive so we need to preheat it
        // i.e. request with larger timeout and with the 
        // progress bar shown to the user to avoid UI lag
        Exception ex = null;
        var res = WindowHelper.LongRunningTask(() => PreheatInstance(instance, out ex), "Starting Sitecore", mainWindow, 
          "Sitecore is being initialized", 
          "It may take up to a few minutes on large solutions or slow machines.", 
          true, true, true);
        if (res == null)
        {
          return false;
        }

        // if error happened
        if (ex != null)
        {
          const string cancel = "Cancel";
          const string openLog = "Open SIM log file";
          const string openSitecoreLog = "Open Sitecore log file";
          const string openAnyway = "Open in browser";
          var message = "The instance returned an error. \n\n" + ex.Message;
          Log.Error(ex, message);
          var result = WindowHelper.AskForSelection("Running instance failed", null, message, 
            new[]
            {
              cancel, openLog, openSitecoreLog, openAnyway
            }, mainWindow);
          switch (result)
          {
            case openLog:
              WindowHelper.OpenFile(ApplicationManager.LogsFolder);
              return false;
            case openSitecoreLog:
              OpenCurrentLogFile(instance, mainWindow);
              return false;
            case openAnyway:
              return true;
            default:
              return false;
          }
        }
      }

      return true;
    }

    #endregion

    #region Private methods

    private static bool EnsureAppPoolState([NotNull] Instance instance, [NotNull] Window mainWindow)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      var state = instance.ApplicationPoolState;
      if (state == ObjectState.Stopped || state == ObjectState.Stopping)
      {
        const string cancel = "Cancel";
        const string start = "Start";
        const string skip = "Skip, open anyway";

        var result = WindowHelper.AskForSelection("Instance is stopped", null, "The selected Sitecore instance is stopped. Would you like to start it first?", new[]
        {
          cancel, start, skip
        }, mainWindow, start);

        if (result == null || result == cancel)
        {
          return false;
        }

        if (result == start)
        {
          instance.Start();
        }
      }

      return true;
    }

    // public static void ToggleFavorite(Window mainWindow, Instance instance)
    // {
    // FavoriteManager.ToggleFavorite(instance.Name);
    // }
    [CanBeNull]
    private static string GetLogFileTypes(Window owner, string logsFolderPath)
    {
      const string Suffix = ".txt";
      const string Pattern = "*" + Suffix;
      var files = FileSystem.FileSystem.Local.Directory.GetFiles(logsFolderPath, Pattern);

      var groups = InstanceHelper.GetLogGroups(files);

      if (groups.Any())
      {
        return WindowHelper.AskForSelection("Open current log file", "Choose log file type", "There are several types of log files in Sitecore. Please choose what type do you need?", groups, owner, groups.First(), false, true);
      }

      WindowHelper.ShowMessage("There are no log files", MessageBoxButton.OK, MessageBoxImage.Asterisk);
      return null;
    }

    private static void PreheatInstance(Instance instance, out Exception exception)
    {
      try
      {
        InstanceHelper.StartInstance(instance, null, "long");
        exception = null;
      }
      catch (Exception ex)
      {
        exception = ex;
      }
    }

    #endregion
  }
}