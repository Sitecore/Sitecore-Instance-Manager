using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using SIM.Base;
using SIM.Instances;

namespace SIM.Tool.Base
{
  public static class InstanceHelper
  {

    public static void BrowseInstance([NotNull] Instance instance, [NotNull] string virtualPath, bool isFrontEnd)
    {
      Assert.ArgumentNotNull(virtualPath, "virtualPath");
      Assert.ArgumentNotNull(instance, "instance");

      string url = instance.GetUrl();
      if (!String.IsNullOrEmpty(url))
      {
        url += '/' + virtualPath.TrimStart('/');
        OpenInBrowser(url, isFrontEnd);
      }
    }

    public static bool PreheatInstance(Instance instance, Window mainWindow, bool ignoreAdvancedSetting = false)
    {
      if (!WindowHelper.AppSettings.AppPreheatEnabled.Value && !ignoreAdvancedSetting)
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
        var res = LongRunningTask(() => PreheatInstance(instance, out ex), "Starting Sitecore", mainWindow,
                                               "Sitecore is being initialized",
                                               "It may take up to a few minutes on large solutions or slow machines.",
                                               true, true);
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
          Log.Error(message, typeof(WindowHelper), ex);
          var result = AskForSelection("Running instance failed", null, message,
                                                    new[] { cancel, openLog, openSitecoreLog, openAnyway }, mainWindow);
          switch (result)
          {
            case openLog:
              OpenFile(Log.LogFilePath);
              return false;
            case openSitecoreLog:
              OpenCurrentLogFile(instance);
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

    public static void OpenCurrentLogFile(Instance instance)
    {
      string dataFolderPath = instance.DataFolderPath;
      FileSystem.Local.Directory.AssertExists(dataFolderPath, "The data folder ({0}) of the {1} instance doesn't exist".FormatWith(dataFolderPath, instance.Name));
      var logs = instance.LogsFolderPath;
      var logFilePath = instance.CurrentLogFilePath;
      Assert.IsNotNull(logFilePath, "There is no log files in the {0} folder".FormatWith(logs));

      string logviewer = WindowHelper.AppSettings.AppToolsLogViewer.Value;
      if (!String.IsNullOrEmpty(logviewer))
      {
        var fileSystemWatcher = new FileSystemWatcher(logs)
        {
          Filter = "log*.txt",
          IncludeSubdirectories = false
        };

        bool ignore = false;

        var process = Process.Start(logviewer, logFilePath);
        if (process == null) return;

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
            logFilePath = instance.CurrentLogFilePath;
            process = Process.Start(logviewer, logFilePath);
          }
          catch
          {
            fileSystemWatcher.EnableRaisingEvents = false;
          }
        };

        fileSystemWatcher.EnableRaisingEvents = true;
      }
      else
      {
        WindowHelper.OpenFile(logFilePath);
      }
    }

    public static void OpenInBrowserAsAdmin(Instance instance, string pageUrl = null)
    {
      WindowHelper.AuthenticationHelper.LoginAsAdmin(instance, pageUrl);
    }
  }
}
